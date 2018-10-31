using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using CarHunters.Core.Common.Models;

namespace CarHunters.Core.Units.ML.Services.Abstractions
{
    public interface ITopCarFeaturesDetector
    {
        Task FeedFrame(FrameEntry frame, RectangleF carBB, bool isFirst);
        List<CarFeatures> TopKFeatures(int K);
        FrameEntry LastCarImage { get; }
        event EventHandler<LogParams> LogChanged;
    }
}
