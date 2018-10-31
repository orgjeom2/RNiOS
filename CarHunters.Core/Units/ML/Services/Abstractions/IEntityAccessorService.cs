using System;
using CarHunters.Core.Common.Models;
using CarHunters.Core.PlatformAbstractions;

namespace CarHunters.Core.Units.ML.Services.Abstractions
{
    public interface IEntityAccessorService
    {
        IPlatformFactoryService Factory { get; }
        IImageHelpers Helpers { get; }
        IPreliminaryFrameClassifier FrameClassifier { get; }
        IVehicleDetector VehicleDetector { get; }
        ICarBBoxDetector CarBBoxDetector { get; }
        ITopCarFeaturesDetector TopFeaturesDetector { get; }
        ICarFeatureClassifier CarModelClassifier { get; }
        ICarFeatureClassifier CarColorClassifier { get; }
        IObjectTracker ObjectTracker { get; }

        void StopEntities();
        void ResumeEntities();

        event EventHandler<LogParams> LogChanged;
    }
}
