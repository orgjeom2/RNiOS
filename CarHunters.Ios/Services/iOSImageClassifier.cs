using System;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using CarHunters.Core.PlatformAbstractions;

using Vision;
using CoreML;
using Foundation;
using CoreVideo;

namespace CarHunters.Ios.Helpers
{
    public class iOSImageClassifier : IImageClassifier
    {
        private VNCoreMLModel _VNMLModel;
        private readonly int _outSize;

        public TimeSpan PreprocessTime { get; private set; }
        public TimeSpan InferenceTime { get; private set; }
        public TimeSpan PostprocessTime { get; private set; }

        public iOSImageClassifier(string modelName, int outSize)
        {
            _outSize = outSize;

            var assetPath = NSBundle.MainBundle.GetUrlForResource(modelName, "mlmodelc");
            var mlModel = MLModel.Create(assetPath, out NSError mlError);
            if (mlError == null)
            {
                _VNMLModel = VNCoreMLModel.FromMLModel(mlModel, out mlError);
            }
        }

        public async Task<float[]> Classify(object image)
        {
            PreprocessTime = TimeSpan.FromTicks(0);

            var startInferTime = DateTimeOffset.UtcNow;

            var multiarr = await NNHelpers.PlayMN(_VNMLModel, image as CVPixelBuffer);

            InferenceTime = DateTimeOffset.UtcNow - startInferTime;

            // ------------------------------------------- //

            var startPostprocessTime = DateTimeOffset.UtcNow;

            double[] newfloat = new double[_outSize];
            Marshal.Copy(multiarr.DataPointer, newfloat, 0, newfloat.Length);
            multiarr.Dispose();

            var probs = newfloat.Select(x => (float)x).ToArray();

            PostprocessTime = DateTimeOffset.UtcNow - startPostprocessTime;

            // ------------------------------------------- //

            return probs;
        }
    }
}