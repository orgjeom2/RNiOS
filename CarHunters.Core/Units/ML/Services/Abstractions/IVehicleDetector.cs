using System;
using System.Drawing;
using CarHunters.Core.Common.Models;

namespace CarHunters.Core.Units.ML.Services.Abstractions
{
    public interface IVehicleDetector
    {
        int MinRunDelay { get; }
        void Run();
        FrameEntry LastFrame { get; set; }
        BboxParams DetectedVehicle { get; }
        event EventHandler<BboxParams> VehicleFound;
    }
}