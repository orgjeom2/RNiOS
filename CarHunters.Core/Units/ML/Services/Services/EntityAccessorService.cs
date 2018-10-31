using CarHunters.Core.Units.ML.Services.Abstractions;
using MvvmCross;
using CarHunters.Core.PlatformAbstractions;
using System.Timers;
using System;
using CarHunters.Core.Common.Models;
using System.Drawing;

namespace CarHunters.Core.Units.ML.Services.Services
{
    public class EntityAccessorService : IEntityAccessorService
    {
        public IPlatformFactoryService Factory { get; private set;  }
        public IPreliminaryFrameClassifier FrameClassifier { get; private set; }
        public IVehicleDetector VehicleDetector { get; private set; }
        public ICarBBoxDetector CarBBoxDetector { get; private set; }
        public ITopCarFeaturesDetector TopFeaturesDetector { get; private set; }
        public ICarFeatureClassifier CarModelClassifier { get; private set; }
        public ICarFeatureClassifier CarColorClassifier { get; private set; }
        public IObjectTracker ObjectTracker { get; private set; }
        public IImageHelpers Helpers { get; private set; }

        public event EventHandler<LogParams> LogChanged;

        private Timer _vdTimer;
        private bool _isStopped;

        public EntityAccessorService()
        {
            Factory = Mvx.IoCProvider.Resolve<IPlatformFactoryService>();
            Helpers = Factory.CreateImageHelpers();
            ObjectTracker = new ObjectTracker(this);
            FrameClassifier = new PreliminaryFrameClassifier(this);
            CarBBoxDetector = new CarBBoxDetector(this);
            VehicleDetector = new VehicleDetector(this);
            CarModelClassifier = new CarModelClassifier(this);
            CarColorClassifier = new CarColorClassifier(this);
            TopFeaturesDetector = new TopCarFeaturesDetector(this);

            SetupTimers();
            ResumeEntities();
        }

        public void StopEntities()
        {
            if (_isStopped) return;

            _isStopped = true;
            _vdTimer.Enabled = false;
            ObjectTracker.Release();
        }

        public void ResumeEntities()
        {
            LogChanged?.Invoke(this, new LogParams()
            {
                ShowIdx = 0,
                Color = Color.Violet,
                Text = $"App version: {Helpers.AppVersion()}"
            });

            // --------------------------- //

            if (!_isStopped) return;

            _isStopped = false;
            _vdTimer.Enabled = true;
        }

        private void SetupTimers()
        {
            _isStopped = false;

            _vdTimer = new Timer(33);
            _vdTimer.Elapsed += (sender, e) => VehicleDetector.Run();
            _vdTimer.AutoReset = true;
            _vdTimer.Enabled = true;
        }
    }
}