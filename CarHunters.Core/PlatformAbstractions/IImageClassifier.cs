using System;
using System.Threading.Tasks;

namespace CarHunters.Core.PlatformAbstractions
{
    public interface IImageClassifier
    {
        Task<float[]> Classify(object image);
        TimeSpan PreprocessTime { get; }
        TimeSpan InferenceTime { get; }
        TimeSpan PostprocessTime { get; }
    }
}
