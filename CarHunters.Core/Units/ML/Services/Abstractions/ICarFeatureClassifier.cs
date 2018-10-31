using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CarHunters.Core.Common.Models;

namespace CarHunters.Core.Units.ML.Services.Abstractions
{
    public interface ICarFeatureClassifier
    {
        int InputSize();
        int OutputSize();

        Task Classify(object image);

        List<float> Probabilities { get; }
        List<string> Labels { get; }
        string BestLabel { get; }
        int BestLabelId { get; }
        float BestProbability { get; }
        List<string> LabelDbIds { get; }

        TimeSpan PreprocessTime { get; }
        TimeSpan InferenceTime { get; }
        TimeSpan PostprocessTime { get; }
    }
}