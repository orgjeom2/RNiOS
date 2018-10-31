using CarHunters.Ios.Views.Abstract;
using CarHunters.Core.ViewModels;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Binding.BindingContext;
using CarHunters.Core.Common.Converters;
using System.Drawing;
using CoreAnimation;
using UIKit;
using CarHunters.Ios.Helpers;
using CoreGraphics;
using CarHunters.Ios.Converters;

namespace CarHunters.Ios.Views
{
    [MvxChildPresentation]
    public partial class HuntView : BaseView<HuntViewModel>
    {
        protected override void DoBind()
        {
            base.DoBind();

            var set = this.CreateBindingSet<HuntView, HuntViewModel>();
            set.Bind(notAvailableView.OpenSettingsButton).To(vm => vm.OpenSettingsCommand);
            set.Bind(notAvailableView.GoBackButton).To(vm => vm.CloseCommand);
            set.Bind(notAvailableView).For(v => v.Hidden).To(vm => vm.CameraAccessGranted);
            set.Bind(cameraControlsView.GoBackButton).To(vm => vm.CloseCommand);
            set.Bind(cameraControlsView).For(v => v.Hidden).To(vm => vm.CameraAccessGranted).WithConversion(nameof(BoolInversionConverter));
            set.Bind(this).For(nameof(CameraAccessGranted)).To(vm => vm.CameraAccessGranted);
            set.Bind(this).For(nameof(BoundingBox)).To(vm => vm.BoundingBox);
            set.Bind(this).For(nameof(BoundingBoxColor)).To(vm => vm.BoundingBoxColor).WithConversion(nameof(CustomColorToUiColorConverter));
            set.Bind(this).For(nameof(DetectedCarBbox)).To(vm => vm.DetectedCarBbox);
            set.Bind(this).For(nameof(DetectedCarBboxColor)).To(vm => vm.DetectedCarBoxColor).WithConversion(nameof(CustomColorToUiColorConverter));
            set.Bind(_source).For(v => v.ItemsSource).To(vm => vm.ItemsSource);
            set.Apply();
        }

        RectangleF? _detectedCarBbox;
        public RectangleF? DetectedCarBbox
        {
            get => _detectedCarBbox;
            set 
            {
                _detectedCarBbox = value;
                if(value == null)
                {
                    DestroyCarBbox();
                }
                else
                {
                    DrawCarBbox();
                }
            }
        }

        RectangleF? _boundingBox;
        public RectangleF? BoundingBox
        {
            get => _boundingBox;
            set
            {
                _boundingBox = value;
                if(value == null)
                {
                    DestroyTrackerBbox();
                }
                else
                {
                    DrawTrackerBbox();
                }
            }
        }
        public UIColor BoundingBoxColor { get; set; }
        public UIColor DetectedCarBboxColor { get; set; }

        void DestroyCarBbox()
        {
            _carBbox?.RemoveFromSuperLayer();
            _carBbox?.Dispose();
            _carBbox = null;
        }

        void DrawCarBbox()
        {
            if (_carBbox == null)
            {
                _carBbox = new CAShapeLayer();
                View.Layer.AddSublayer(_carBbox);
            }

            _carBbox.FillColor = UIColor.Clear.CGColor;
            _carBbox.StrokeColor = DetectedCarBboxColor.CGColor;
            _carBbox.LineWidth = 3;
            _carBbox.AllowsEdgeAntialiasing = true;

            _carBbox.Path = UIBezierPath.FromRoundedRect(CameraHelper.GetRectForBbox(DetectedCarBbox.Value), 3).CGPath;
        }

        void DestroyTrackerBbox()
        {
            _trackerBbox?.RemoveFromSuperLayer();
            _trackerBbox?.Dispose();
            _trackerBbox = null;
        }

        void DrawTrackerBbox()
        {
            if (_trackerBbox == null)
            {
                _trackerBbox = new CAShapeLayer();
                View.Layer.AddSublayer(_trackerBbox);
            }

            var strokeWidth = 4;

            _trackerBbox.FillColor = UIColor.Clear.CGColor;
            _trackerBbox.StrokeColor = BoundingBoxColor.CGColor;
            _trackerBbox.LineWidth = strokeWidth;
            _trackerBbox.AllowsEdgeAntialiasing = true;
            
            var linewidth = 40f;
            var drawRect = CameraHelper.GetRectForBbox(BoundingBox.Value);
            if (drawRect.Width < drawRect.Height)
            {
                if (linewidth > drawRect.Width / 5)
                {
                    linewidth = (float)drawRect.Width / 5;
                }
            }
            else
            {
                if (linewidth > drawRect.Height / 5)
                {
                    linewidth = (float)drawRect.Height / 5;
                }
            }

            UIBezierPath linePath = new UIBezierPath();
            var halfStroke = strokeWidth / 2;
            linePath.MoveTo(new CGPoint(drawRect.X - halfStroke, drawRect.Y));
            linePath.AddLineTo(new CGPoint(drawRect.X + linewidth, drawRect.Y));
            linePath.MoveTo(new CGPoint(drawRect.X + drawRect.Width - linewidth, drawRect.Y));
            linePath.AddLineTo(new CGPoint(drawRect.X + drawRect.Width + halfStroke, drawRect.Y));

            linePath.MoveTo(new CGPoint(drawRect.X - halfStroke, drawRect.Y + drawRect.Height));
            linePath.AddLineTo(new CGPoint(drawRect.X + linewidth, drawRect.Y + drawRect.Height));
            linePath.MoveTo(new CGPoint(drawRect.X + drawRect.Width - linewidth, drawRect.Y + drawRect.Height));
            linePath.AddLineTo(new CGPoint(drawRect.X + drawRect.Width + halfStroke, drawRect.Y + drawRect.Height));

            linePath.MoveTo(new CGPoint(drawRect.X + drawRect.Width, drawRect.Y - halfStroke));
            linePath.AddLineTo(new CGPoint(drawRect.X + drawRect.Width, drawRect.Y + linewidth));
            linePath.MoveTo(new CGPoint(drawRect.X + drawRect.Width, drawRect.Y + drawRect.Height - linewidth));
            linePath.AddLineTo(new CGPoint(drawRect.X + drawRect.Width, drawRect.Y + drawRect.Height + halfStroke));

            linePath.MoveTo(new CGPoint(drawRect.X, drawRect.Y - halfStroke));
            linePath.AddLineTo(new CGPoint(drawRect.X, drawRect.Y + linewidth));
            linePath.MoveTo(new CGPoint(drawRect.X, drawRect.Y + drawRect.Height - linewidth));
            linePath.AddLineTo(new CGPoint(drawRect.X, drawRect.Y + drawRect.Height + halfStroke));

            _trackerBbox.Path = linePath.CGPath;
        }

        bool cameraAccessGranted;
        public bool CameraAccessGranted
        {
            get => cameraAccessGranted;
            set
            {
                cameraAccessGranted = value;
                if(value)
                {
                    EnableCameraView();
                }
                else
                {
                    DisableCameraView();
                }
            }
        }
    }
}

