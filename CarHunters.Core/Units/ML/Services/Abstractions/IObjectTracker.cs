using System;
using System.Drawing;
using System.Threading.Tasks;
using CarHunters.Core.Common.Models;

namespace CarHunters.Core.Units.ML.Services.Abstractions
{
    public interface IObjectTracker
    {
        Task NextFrame(FrameEntry frame);
        Task TrackNewObject(FrameEntry frame, RectangleF? bbox);
        BboxParams TrackedCar { get; }
        event EventHandler<BboxParams> BboxChanged;
        event EventHandler<IdentifiedCarParams> CarIdentificationFinished;
        event EventHandler<LogParams> LogChanged;
        void Release();
    }
}