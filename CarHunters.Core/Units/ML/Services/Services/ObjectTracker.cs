using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using CarHunters.Core.Units.ML.Services.Abstractions;
using CarHunters.Core.Common.Models;

namespace CarHunters.Core.Units.ML.Services.Services
{
    public class ObjectTracker : IObjectTracker
    {
        // common thresholds
        private static readonly long MAX_HISTORY_DURATION = 2000;
        private static readonly long DETECTION_ABSENCE_THRESHOLD = 1000;
        private static readonly double CANDIDATE_VS_OLD_IOU_BIG_THRESHOLD = 0.9;
        private static readonly double CANDIDATE_VS_OLD_IOU_LITTLE_THRESHOLD = 0.4;
        private static readonly double MIN_CORRELATION = 0.3;

        // fields related to BBt state
        public enum State { EMPTY, WEAK, IDENT_FREE, IDENT_BUSY, IDENT_DONE, UNSTABLE, OUT };
        private static readonly int BBOX_HEIGHT_THRESHOLD = 160;
        private static readonly int BBOX_WIDTH_THRESHOLD = 190;
        private static readonly double MIN_STABLE_IOU = 0.5;
        private static readonly long TRANSIT_TO_IDENTIFICATION_TIME = 700;
        private static readonly long STABILIZING_TIME = 800;
        private static readonly long IDENTIFICATION_DELAY_TIME = 500;
        private State _state = State.EMPTY;
        private int _identProgress = 0;
        private long _startStabilizingTime = 0;
        private long _startBeingBigTime = 0;
        private long _lastIdentificationTime = 0;
        private bool _isTrackingStable = false;
        private RectangleF? _pivotBox = null;
        private RectangleF? _BBt = null;
        private bool _caughtNewCar = false;

        // main fields
        private readonly SemaphoreSlim _trackerSemaphore = new SemaphoreSlim(1, 1);
        private BboxParams _trackedCar;
        private EntityAccessorService _accessor;
        private bool _isCppTrackerCreated = false;
        private long _trackedObjectHandle = -1;
        private long _lastPositiveDetectionTimestamp = 0;
        private SortedList<long, RectangleF> _history = new SortedList<long, RectangleF>();

        public ObjectTracker(EntityAccessorService accessor)
        {
            TrackedCar = null;
            _accessor = accessor;
        }

        public event EventHandler<BboxParams> BboxChanged;
        public event EventHandler<LogParams> LogChanged;
        public event EventHandler<IdentifiedCarParams> CarIdentificationFinished;

        private double NextFrameMilliseconds { get; set; }
        private double BgraToGrayMilliseconds { get; set; }
        private double ResizeGrayMilliseconds { get; set; }
        private double NewObjectMilliseconds { get; set; }

        public BboxParams TrackedCar
        {
            get => _trackedCar;
            set
            {
                _trackedCar = value;
                BboxChanged?.Invoke(this, value);
            }
        }

        public async Task NextFrame(FrameEntry frame)
        {
            if (frame == null) return;

            await _trackerSemaphore.WaitAsync();

            if (!_isCppTrackerCreated)
            {
                NativeMethods.CreateObjectTracker(frame.Width / 2, frame.Height / 2);
                _isCppTrackerCreated = true;
                _state = State.EMPTY;
                _BBt = null;
            }

            DateTimeOffset current = DateTimeOffset.UtcNow;
            byte[] gray = new byte[frame.Width * frame.Height];
            NativeMethods.BgraToGray(frame.Frame, frame.Width, frame.Height, gray);
            var bgraToGrayTime = TimeSpan.FromTicks(DateTimeOffset.UtcNow.Ticks - current.Ticks);
            BgraToGrayMilliseconds = bgraToGrayTime.TotalMilliseconds;

            current = DateTimeOffset.UtcNow;
            byte[] scaled = new byte[frame.Width * frame.Height / 4];
            NativeMethods.DownscaleGrayImageTwice(gray, frame.Width, frame.Height, scaled);
            var resizeGrayTime = TimeSpan.FromTicks(DateTimeOffset.UtcNow.Ticks - current.Ticks);
            ResizeGrayMilliseconds = resizeGrayTime.TotalMilliseconds;

            current = DateTimeOffset.UtcNow;
            NativeMethods.NextFrame(scaled, frame.TimeStamp.Ticks / TimeSpan.TicksPerMillisecond);
            var nextFrameTime = TimeSpan.FromTicks(DateTimeOffset.UtcNow.Ticks - current.Ticks);
            NextFrameMilliseconds = nextFrameTime.TotalMilliseconds;

            Notify();

            // update BBt and BBt history 
            RemoveOutdatedBoxes(DateTimeOffset.UtcNow.Ticks / TimeSpan.TicksPerMillisecond);
            if (_trackedObjectHandle != -1)
            {
                double correlation = NativeMethods.GetCurrentCorrelation(_trackedObjectHandle);
                if (correlation > MIN_CORRELATION)
                {
                    float[] bb = new float[4];
                    NativeMethods.GetCurrentTrackedBBox(_trackedObjectHandle, bb);
                    bb[0] /= frame.Height / 2;
                    bb[1] /= frame.Width / 2;
                    bb[2] /= frame.Height / 2;
                    bb[3] /= frame.Width / 2;
                    _BBt = RectangleF.FromLTRB(bb[1], bb[0], bb[3], bb[2]);
                    if (IsNearFrameBorder(_BBt.Value, frame))
                        _BBt = null;
                    else
                        _history.Add(frame.TimeStamp.Ticks / TimeSpan.TicksPerMillisecond,
                                     _BBt.Value);
                }
                else
                {
                    // maybe, stop tracking in this case?
                    _BBt = null;
                }
            }
            else
            {
                _BBt = null;
            }

            UpdateTrackingState(frame);

            _trackerSemaphore.Release();
        }

        private void RemoveOutdatedBoxes(long timestamp)
        {
            while (_history.Count > 0 && timestamp - _history.First().Key > MAX_HISTORY_DURATION)
                _history.RemoveAt(0);
        }

        private double IoU(RectangleF a, RectangleF b)
        {
            RectangleF intersection = RectangleF.Intersect(a, b);
            if (intersection.IsEmpty) return 0;
            double intersectArea = intersection.Width * intersection.Height;
            double unionArea = a.Width * a.Height + b.Width * b.Height - intersectArea;
            return intersectArea / (unionArea + 1e-5);
        }

        private bool IsBBoxTooSmall(RectangleF bbox, FrameEntry frame)
        {
            float bbWidth = bbox.Width * frame.Width;
            float bbHeight = bbox.Height * frame.Height;
            if (bbWidth < bbHeight)
            {
                var tmp = bbWidth;
                bbWidth = bbHeight;
                bbHeight = tmp;
            }
            return bbWidth < BBOX_WIDTH_THRESHOLD || bbHeight < BBOX_HEIGHT_THRESHOLD;
        }

        private void UpdateTrackingState(FrameEntry frame)
        {
            long timeNow = DateTimeOffset.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
            // now transiting to another activity (e.g. to CarInfoActivity)
            if (_state == State.OUT)
            {
                TrackedCar = new BboxParams
                {
                    Bbox = _BBt,
                    Color = Color.Green
                };
                return;
            }
            // check identification results
            if (_state == State.IDENT_DONE)
            {
                Debug.WriteLine("+++INFO: Identification is done");
                TrackedCar = new BboxParams
                {
                    Bbox = _BBt,
                    Color = Color.Green
                };
                _state = State.OUT;

                var top3 = _accessor.TopFeaturesDetector.TopKFeatures(3);
                var carImage = _accessor.TopFeaturesDetector.LastCarImage;
                var eventParams = new IdentifiedCarParams()
                {
                    TopResults = top3,
                    CroppedCarImage = carImage,
                    //GeoLocation = _accessor.GeoListener.LastLocation
                };
                CarIdentificationFinished?.Invoke(this, eventParams);
                for (int idx = 0; idx < top3.Count; idx++)
                    Debug.WriteLine($"{idx} - {top3[idx].ModelName} {top3[idx].ModelProbability}, " +
                                    $"{top3[idx].ColorName} {top3[idx].ColorProbability}");
                return;
            }
            // no tracked object
            if (_BBt == null)
            {
                TrackedCar = new BboxParams
                {
                    Bbox = null
                };
                _state = State.EMPTY;
                return;
            }
            // new car is caught OR no plate in bbox OR object is too small
            if (_caughtNewCar || /* !mMainActivity.getVehicleDetector().isPlateFound() || */
                    IsBBoxTooSmall(_BBt.Value, frame))
            {
                //if (mCaughtNewCar)
                //    Log.d("+++INFO", "Caught new car");
                TrackedCar = new BboxParams
                {
                    Bbox = _BBt,
                    Color = Color.White
                };
                _state = State.WEAK;
                _startBeingBigTime = timeNow;
                return;
            }
            // transit from EMPTY
            if (_state == State.EMPTY)
            {
                Debug.WriteLine("+++INFO: EMPTY to WEAK");
                TrackedCar = new BboxParams
                {
                    Bbox = _BBt,
                    Color = Color.White
                };
                _state = State.WEAK;
                _startBeingBigTime = timeNow;
                return;
            }
            // object is big enough and still transiting to identification state
            if (_state == State.WEAK && timeNow - _startBeingBigTime < TRANSIT_TO_IDENTIFICATION_TIME)
            {
                TrackedCar = new BboxParams
                {
                    Bbox = _BBt,
                    Color = Color.White
                };
                return;
            }
            // transit from WEAK
            if (_state == State.WEAK)
            {
                Debug.WriteLine("+++INFO: From WEAK to IDENT_FREE");
                TrackedCar = new BboxParams
                {
                    Bbox = _BBt,
                    Color = Color.Yellow
                };
                _state = State.IDENT_FREE;
                _isTrackingStable = true;
                _identProgress = 0;
                _pivotBox = _BBt;
                _lastIdentificationTime = timeNow - 2 * IDENTIFICATION_DELAY_TIME;
                // no return statement here
            }
            // tracking becomes unstable due to phone twitching
            if (_state != State.IDENT_DONE && IoU(_pivotBox.Value, _BBt.Value) < MIN_STABLE_IOU)
            {
                _isTrackingStable = false;
                // no return statement here
            }
            // still stabilizing
            if (_state == State.UNSTABLE && timeNow - _startStabilizingTime < STABILIZING_TIME)
            {
                TrackedCar = new BboxParams
                {
                    Bbox = _BBt,
                    Color = Color.Red
                };
                return;
            }
            // make new attempt to stabilize
            if (_state == State.UNSTABLE && !_isTrackingStable)
            {
                Debug.WriteLine("+++INFO: UNSTABLE to UNSTABLE");
                TrackedCar = new BboxParams
                {
                    Bbox = _BBt,
                    Color = Color.Red
                };
                _isTrackingStable = true;
                _identProgress = 0;
                _pivotBox = _BBt;
                _startStabilizingTime = timeNow;
                return;
            }
            // identification is busy
            if (_state == State.IDENT_BUSY)
            {
                TrackedCar = new BboxParams
                {
                    Bbox = _BBt,
                    Color = Color.Yellow
                };
                return;
            }
            // delay between consecutive identifications
            if (_state == State.IDENT_FREE && timeNow - _lastIdentificationTime < IDENTIFICATION_DELAY_TIME)
            {
                TrackedCar = new BboxParams
                {
                    Bbox = _BBt,
                    Color = Color.Yellow
                };
                return;
            }
            // start identifying car
            Debug.WriteLine("+++INFO: to IDENT_BUSY");
            TrackedCar = new BboxParams
            {
                Bbox = _BBt,
                Color = Color.Yellow
            };
            _state = State.IDENT_BUSY;
            _lastIdentificationTime = timeNow;
            _identProgress++;
            if (_identProgress == 1)
            {
                _pivotBox = _BBt;
                _isTrackingStable = true;
            }

            IdentifyCarModel(frame);
        }

        private async void IdentifyCarModel(FrameEntry frame)
        {
            Debug.WriteLine($"+++INFO: IdentifyCarModel {_identProgress}");

            if (!_isTrackingStable)
            {
                _state = State.UNSTABLE;
                Debug.WriteLine($"+++INFO: IdentifyCarModel: UNSTABLE");
                return;
            }

            await _trackerSemaphore.WaitAsync();
            RectangleF? bb = null;
            if (_BBt.HasValue)
                bb = _BBt.Value;
            _trackerSemaphore.Release();

            if (bb != null)
                await _accessor.TopFeaturesDetector.FeedFrame(frame, bb.Value, _identProgress == 1);

            await _trackerSemaphore.WaitAsync();
            if (_identProgress < 3)
            {
                TrackedCar = new BboxParams
                {
                    Bbox = _BBt,
                    Color = Color.Yellow
                };
                _state = State.IDENT_FREE;
            }
            else
            {
                if (_isTrackingStable)
                {
                    TrackedCar = new BboxParams
                    {
                        Bbox = _BBt,
                        Color = Color.Green
                    };
                    _state = State.IDENT_DONE;
                }
                else
                {
                    TrackedCar = new BboxParams
                    {
                        Bbox = _BBt,
                        Color = Color.Red
                    };
                    _state = State.UNSTABLE;
                }
            }
            _trackerSemaphore.Release();
        }

        // --------------------------------------------- //

        public async Task TrackNewObject(FrameEntry frame, RectangleF? bbox)
        {
            await _trackerSemaphore.WaitAsync();

            if (_isCppTrackerCreated)
                ConsiderNewObjectToTrack(frame, bbox);

            _trackerSemaphore.Release();
        }

        private bool IsNearFrameBorder(RectangleF rect, FrameEntry frame)
        {
            float margin = 0.02f;
            bool nearLeftRightBorder = rect.Left <= margin || rect.Right >= 1 - margin;
            bool nearTopBottomBorder = rect.Top <= margin || rect.Bottom >= 1 - margin;
            return nearLeftRightBorder || nearTopBottomBorder;
        }

        private void PrepareAndTrack(FrameEntry frame, RectangleF bbox)
        {
            byte[] gray = new byte[frame.Width * frame.Height];
            NativeMethods.BgraToGray(frame.Frame, frame.Width, frame.Height, gray);

            byte[] scaled = new byte[frame.Width * frame.Height / 4];
            NativeMethods.DownscaleGrayImageTwice(gray, frame.Width, frame.Height, scaled);

            DateTimeOffset current = DateTimeOffset.UtcNow;

            if (_trackedObjectHandle != -1)
                NativeMethods.ForgetTrackedObject(_trackedObjectHandle);

            RectangleF bb = bbox;
            bb = RectangleF.FromLTRB(bb.Left * frame.Width / 2, bb.Top * frame.Height / 2,
                                     bb.Right * frame.Width / 2, bb.Bottom * frame.Height / 2);

            _trackedObjectHandle = NativeMethods.TrackObject(
                                        scaled,
                                        frame.TimeStamp.Ticks / TimeSpan.TicksPerMillisecond,
                                        bb.Left, bb.Top, bb.Right, bb.Bottom);

            var newObjectTime = TimeSpan.FromTicks(DateTimeOffset.UtcNow.Ticks - current.Ticks);
            NewObjectMilliseconds = newObjectTime.TotalMilliseconds;

            Notify();

            Debug.WriteLine($"+++INFO: New tracked object id: {_trackedObjectHandle}");
        }

        private void ConsiderNewObjectToTrack(FrameEntry frame, RectangleF? bbox)
        {
            //Debug.WriteLine($"+++INFO: ConsiderNewObjectToTrack called");

            long timeNow = DateTimeOffset.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;

            bool isLastDetectionOutdated =
                    timeNow - _lastPositiveDetectionTimestamp > DETECTION_ABSENCE_THRESHOLD;

            // Nothing to track
            if (bbox == null || IsNearFrameBorder(bbox.Value, frame))
            {
                if (_trackedObjectHandle != -1 && isLastDetectionOutdated)
                {
                    NativeMethods.ForgetTrackedObject(_trackedObjectHandle);
                    _trackedObjectHandle = -1;
                }
                Debug.WriteLine($"+++INFO: NewObject: Nothing to track");
                return;
            }

            _caughtNewCar = false;

            // Nothing was tracked
            if (_trackedObjectHandle == -1 || _BBt == null)
            {
                PrepareAndTrack(frame, bbox.Value);
                _caughtNewCar = true;
                _lastPositiveDetectionTimestamp = timeNow;
                Debug.WriteLine($"+++INFO: NewObject: Nothing was tracked");
                return;
            }

            long frameTimestamp = frame.TimeStamp.Ticks / TimeSpan.TicksPerMillisecond;
            RectangleF? old = null;
            KeyValuePair<long, RectangleF>? entry = _history.FirstOrDefault(x => x.Key >= frameTimestamp);
            if (entry != null)
                old = entry.Value.Value;

            // There is no bbox in the recent history to match with the current detection
            if (old == null)
            {
                PrepareAndTrack(frame, bbox.Value);
                _caughtNewCar = true;
                _lastPositiveDetectionTimestamp = timeNow;
                Debug.WriteLine($"+++INFO: NewObject: No BBt in history");
                return;
            }

            double iou = IoU(old.Value, bbox.Value);
            bool isIntersectionBig = iou > CANDIDATE_VS_OLD_IOU_BIG_THRESHOLD;
            bool isIntersectionLittle = iou < CANDIDATE_VS_OLD_IOU_LITTLE_THRESHOLD;
            // Bbox has been changed sufficiently to update
            if (!isIntersectionBig && isLastDetectionOutdated)
            {
                PrepareAndTrack(frame, bbox.Value);
                if (isIntersectionLittle)
                    _caughtNewCar = true;
                _lastPositiveDetectionTimestamp = timeNow;
                Debug.WriteLine($"+++INFO: NewObject: to update BBt");
            }
        }

        private void Notify()
        {
            var logParams = new LogParams
            {
                ShowIdx = 1,
                Color = Color.Cyan,
                Text = $"ObjectTracker:{Environment.NewLine}" +
                       $" - BGRA to gray: {BgraToGrayMilliseconds:F1}ms{Environment.NewLine}" +
                       $" - Resize gray: {ResizeGrayMilliseconds:F1}ms{Environment.NewLine}" +
                       $" - Next frame: {NextFrameMilliseconds:F1}ms{Environment.NewLine}" +
                       $" - New object: {NewObjectMilliseconds:F1}ms{Environment.NewLine}"
            };
            LogChanged?.Invoke(this, logParams);
        }

        public void Release()
        {
            _trackerSemaphore.Wait();

            if (_trackedObjectHandle != -1)
            {
                NativeMethods.ForgetTrackedObject(_trackedObjectHandle);
                _trackedObjectHandle = -1;
            }
            NativeMethods.ReleaseObjectTracker();
            _isCppTrackerCreated = false;

            _trackerSemaphore.Release();
        }
    }
}
