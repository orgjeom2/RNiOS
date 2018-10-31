using CarHunters.Ios.CustomViews;
using Cirrious.FluentLayouts.Touch;
using AVFoundation;
using CoreMedia;
using Foundation;
using UIKit;
using CarHunters.Ios.Helpers;
using CoreVideo;
using CoreFoundation;
using CarHunters.Ios.CustomControls;
using CoreGraphics;
using CoreAnimation;
using MvvmCross.Platforms.Ios.Binding.Views;
using CarHunters.Ios.Cells;

namespace CarHunters.Ios.Views
{
    partial class HuntView
    {
        CameraNotAvailableView notAvailableView;
        CameraControlsView cameraControlsView;
        AVCaptureSession captureSession;
        AVCaptureDevice captureDevice;
        AVCaptureVideoPreviewLayer captureVideoPreviewLayer;
        DispatchQueue queue;
        CustomOutputRecorder outputRecorder;
        CAShapeLayer _trackerBbox;
        CAShapeLayer _carBbox;
        UITableView _table;
        MvxSimpleTableViewSource _source;

        void EnableCameraView()
        {
            if (captureSession != null)
                return;

            captureSession = new AVCaptureSession
            {
                SessionPreset = AVCaptureSession.Preset1280x720
            };

            captureDevice = AVCaptureDevice.GetDefaultDevice(AVMediaType.Video);
            if (captureDevice == null)
            {
                ReleaseCaptureSession();
                return;
            }

            captureDevice.LockForConfiguration(out NSError error);

            var format = captureDevice.ActiveFormat;
            double epsilon = 0.00000001;

            var desiredFrameRate = 30;
            captureDevice.ActiveVideoMinFrameDuration = new CMTime(1, 15);
            foreach(var range in format.VideoSupportedFrameRateRanges)
            {
                if (range.MinFrameRate <= (desiredFrameRate + epsilon) && range.MaxFrameRate >= (desiredFrameRate - epsilon))
                {
                    var duration = new CMTime(1, desiredFrameRate, 0)
                    {
                        TimeFlags = CMTime.Flags.Valid
                    };
                    var minDuration = new CMTime(1, (int)range.MinFrameRate, 0)
                    {
                        TimeFlags = CMTime.Flags.Valid
                    };
                    captureDevice.ActiveVideoMaxFrameDuration = duration;
                    captureDevice.ActiveVideoMinFrameDuration = duration;
                    break;
                }
            }

            captureDevice.UnlockForConfiguration();

            var input = AVCaptureDeviceInput.FromDevice(captureDevice);
            if (input == null)
            {
                ReleaseCaptureSession();
                ReleaseCaptureDevice();
                return;
            }

            captureSession.AddInput(input);
            captureVideoPreviewLayer = new AVCaptureVideoPreviewLayer(captureSession)
            {
                BackgroundColor = UIColor.Black.CGColor,
                VideoGravity = AVLayerVideoGravity.ResizeAspectFill,
                Frame = UIScreen.MainScreen.Bounds
            };
            captureVideoPreviewLayer.Connection.VideoOrientation = CameraHelper.VideoOrientationFromCurrentDeviceOrientation();

            View.Layer.InsertSublayer(captureVideoPreviewLayer, 0);

            var settings = new CVPixelBufferAttributes
            {
                PixelFormatType = CVPixelFormatType.CV32BGRA
            };
            using (var output = new AVCaptureVideoDataOutput { WeakVideoSettings = settings.Dictionary })
            {
                queue = new DispatchQueue("cameraoutputqueue");
                outputRecorder = new CustomOutputRecorder();
                output.AlwaysDiscardsLateVideoFrames = true;
                output.SetSampleBufferDelegateQueue(outputRecorder, queue);
                captureSession.AddOutput(output);
                var connection = output.ConnectionFromMediaType(AVMediaType.Video);
                if (connection != null)
                {
                    connection.VideoOrientation = CameraHelper.VideoOrientationFromCurrentDeviceOrientation();
                }
            }

            captureSession.StartRunning();
        }

        void DisableCameraView()
        {
            captureSession?.StopRunning();
            ReleaseCaptureSession();
            ReleaseCaptureDevice();
            ReleasePreviewLayer();
        }

        protected override void InitView()
        {
            base.InitView();

            _table = new UITableView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                RowHeight = UITableView.AutomaticDimension,
                EstimatedRowHeight = 44f,
                SeparatorColor = UIColor.Clear,
                BackgroundColor = UIColor.Clear,
                TableFooterView = new UIView(CGRect.Empty),
                TableHeaderView = new UIView(CGRect.Empty)
            };

            _source = new MvxSimpleTableViewSource(_table, typeof(LogItemTableViewCell), LogItemTableViewCell.Key);
            _table.Source = _source;

            notAvailableView = new CameraNotAvailableView
            {
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            cameraControlsView = new CameraControlsView
            {
                TranslatesAutoresizingMaskIntoConstraints = false
            };
        }

        protected override void ConstraintView()
        {
            base.ConstraintView();

            View.AddSubviews(notAvailableView, _table, cameraControlsView);
            View.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            View.AddConstraints(
                notAvailableView.Top().EqualTo().TopOf(View),
                notAvailableView.Leading().EqualTo().LeadingOf(View),
                notAvailableView.Trailing().EqualTo().TrailingOf(View),
                notAvailableView.Bottom().EqualTo().BottomOf(View),

                _table.Top().EqualTo().TopOf(View),
                _table.Bottom().EqualTo().BottomOf(View),
                _table.Leading().EqualTo().LeadingOf(View),
                _table.Trailing().EqualTo().TrailingOf(View),

                cameraControlsView.Bottom().EqualTo(-20).BottomOf(View),
                cameraControlsView.Leading().EqualTo().LeadingOf(View),
                cameraControlsView.Trailing().EqualTo().TrailingOf(View),
                cameraControlsView.Height().EqualTo(100)
            );
        }

        protected override void DoViewDidLoad()
        {
            base.DoViewDidLoad();
            NavigationController.SetNavigationBarHidden(true, true);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (CameraAccessGranted)
                EnableCameraView();
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            NavigationController.SetNavigationBarHidden(false, true);
            DisableCameraView();
        }

        void ReleaseCaptureSession()
        {
            if (captureSession == null)
                return;

            captureSession.Dispose();
            captureSession = null;
        }

        void ReleaseCaptureDevice()
        {
            if (captureDevice == null)
                return;

            captureDevice.Dispose();
            captureDevice = null;
        }

        void ReleasePreviewLayer()
        {
            if (captureVideoPreviewLayer == null)
                return;

            captureVideoPreviewLayer.Dispose();
            captureVideoPreviewLayer = null;
        }
    }
}
