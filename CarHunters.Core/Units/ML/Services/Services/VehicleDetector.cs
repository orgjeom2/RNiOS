using System;
using System.Threading;
using System.Drawing;
using CarHunters.Core.Units.ML.Services.Abstractions;
using CarHunters.Core.Common.Models;
using CarHunters.Core.PlatformAbstractions;

namespace CarHunters.Core.Units.ML.Services.Services
{
    public class VehicleDetector : IVehicleDetector
    {
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
        private IEntityAccessorService _accessor;
        private DateTimeOffset _prevRunTimestamp;
        private BboxParams _detectedVehicle;
        public event EventHandler<BboxParams> VehicleFound;
        public int MinRunDelay => 400;

        public FrameEntry LastFrame { get; set; }

        public BboxParams DetectedVehicle
        {
            get => _detectedVehicle;
            set
            {
                _detectedVehicle = value;
                VehicleFound?.Invoke(this, value);
            }
        }

        public VehicleDetector(IEntityAccessorService accessor)
        {
            _accessor = accessor;
            LastFrame = null;
        }

        public async void Run()
        {
            if (LastFrame == null) return;

            var timePassed = TimeSpan.FromTicks(DateTimeOffset.UtcNow.Ticks) - TimeSpan.FromTicks(_prevRunTimestamp.Ticks);
            if (timePassed.TotalMilliseconds < MinRunDelay)
            {
                return;
            }

            if (_semaphoreSlim.CurrentCount == 0)
            {
                return;
            }

            await _semaphoreSlim.WaitAsync();

            _prevRunTimestamp = DateTimeOffset.UtcNow;

            var frame = LastFrame;

            try
            {
                var orientNumber = _accessor.Helpers.DeviceOrientationAsNumber();
                var rotated = _accessor.Helpers.RotateImage(frame, orientNumber);
                //_accessor.DumpService.SaveFrame(rotated);
                var image = _accessor.Helpers.ToNativeImage(rotated);
                float[] bbox = await _accessor.CarBBoxDetector.InferMainBBox(image);
                if (image is IDisposable disposable)
                {
                    disposable.Dispose();
                }

                if (bbox == null)
                {
                    DetectedVehicle = new BboxParams
                    {
                        Bbox = null
                    };
                }
                else
                {
                    var rect = RectangleF.FromLTRB(bbox[1], bbox[0], bbox[3], bbox[2]);
                    DetectedVehicle = new BboxParams
                    {
                        Bbox = _accessor.Helpers.RotateBBoxClockwise(rect, orientNumber),
                        Color = Color.Magenta
                    };
                }
                await _accessor.ObjectTracker.TrackNewObject(frame, DetectedVehicle.Bbox);
            }
            catch
            {
                // ignore all exceptions
            }

            _semaphoreSlim.Release();
        }
    }
}