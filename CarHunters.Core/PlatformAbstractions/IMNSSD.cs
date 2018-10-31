using System;
using System.Threading.Tasks;

namespace CarHunters.Core.PlatformAbstractions
{
    public interface IMNSSD
    {
        Task<Tuple<float[], float[]>> PlayMNSSD(object pixelBuffer, int labelsCount, int bbCount);
        TimeSpan PreprocessTime { get; }
        TimeSpan InferenceTime { get; }
        TimeSpan PostprocessTime { get; }
    }
}
