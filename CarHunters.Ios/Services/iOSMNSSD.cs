using System;
using System.IO;
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
    public class iOSMNSSD : IMNSSD
    {
        private VNCoreMLModel _VNMLModel;

        public TimeSpan PreprocessTime { get; private set; }
        public TimeSpan InferenceTime { get; private set; }
        public TimeSpan PostprocessTime { get; private set; }

        public iOSMNSSD()
        {
            var assetPath = NSBundle.MainBundle.GetUrlForResource("ssd_mobilenet_feature_extractor", "mlmodelc");
            var mlModel = MLModel.Create(assetPath, out NSError mlError);
            if(mlError == null)
            {
                _VNMLModel = VNCoreMLModel.FromMLModel(mlModel, out mlError);
            }
        }

        public async Task<Tuple<float[],float[]>> PlayMNSSD(object pixelBuffer, int labelsCount, int bbCount)
        {
            var image = pixelBuffer as CVPixelBuffer;

            PreprocessTime = TimeSpan.FromTicks(0);

            var startInferTime = DateTimeOffset.UtcNow;

            var tcs = new TaskCompletionSource<MLMultiArray[]>();
            if(_VNMLModel == null)
            {
                tcs.TrySetCanceled();
            }

            var request = new VNCoreMLRequest(_VNMLModel, (response, e) =>
            {

                if (e != null)
                {
                    tcs.SetException(new NSErrorException(e));
                }
                else
                {
                    var results = response.GetResults<VNCoreMLFeatureValueObservation>();
                    var probs = results[0].FeatureValue;
                    var bboxes = results[1].FeatureValue;
                    if (bboxes != null && probs != null)
                    {
                        tcs.SetResult(new MLMultiArray[]{ probs.MultiArrayValue, bboxes.MultiArrayValue });
                        bboxes.Dispose();
                        probs.Dispose();
                    }
                    else
                    {
                        tcs.SetCanceled();
                    }
                }
            });
            request.ImageCropAndScaleOption = VNImageCropAndScaleOption.ScaleFill;

            var requestHandler = new VNImageRequestHandler(image, new NSDictionary());

            requestHandler.Perform(new[] { request }, out NSError error);

            var outs = await tcs.Task;

            if (error != null)
                throw new NSErrorException(error);

            InferenceTime = DateTimeOffset.UtcNow - startInferTime;

            // Debug.WriteLine($"MNSSD infer: {InferenceTime.TotalMilliseconds}");

            // ------------------------------------------- //

            var startPostprocTime = DateTimeOffset.UtcNow;

            var outProbs = new double[bbCount * labelsCount];
            var outBBoxes = new double[bbCount * 4];
            Marshal.Copy(outs[0].DataPointer, outProbs, 0, outProbs.Length);
            Marshal.Copy(outs[1].DataPointer, outBBoxes, 0, outBBoxes.Length);
            outs[0].Dispose();
            outs[1].Dispose();

            float[] probsAll = Array.ConvertAll(outProbs, x => (float)x);
            float[] bboxesAll = Array.ConvertAll(outBBoxes, x => (float)x);

            PostprocessTime = DateTimeOffset.UtcNow - startPostprocTime;

            // Debug.WriteLine($"MNSSD postproc: {InferenceTime.TotalMilliseconds}");

            // ------------------------------------------- //

            return Tuple.Create(probsAll, bboxesAll);
        }
    }
}