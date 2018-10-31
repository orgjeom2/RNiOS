using System.Linq;
using System.Threading.Tasks;
using CarHunters.Core.Common.Models;

using CoreML;
using Vision;
using CoreVideo;
using Foundation;

namespace CarHunters.Ios.Helpers
{
    public static class NNHelpers
    {
        // you will need to explicitly dispose the returned CVPixelBuffer after using it
        public static CVPixelBuffer ToCVPixelBuffer(FrameEntry frame)
        {
            var bytesPerPixel = 4;
            var bytesPerRow = frame.Width * bytesPerPixel;
            var attr = new CVPixelBufferAttributes()
            {
                PixelFormatType = CVPixelFormatType.CV32BGRA,
                AllocateWithIOSurface = true,
                MetalCompatibility = true,
                OpenGLCompatibility = true,
                CGImageCompatibility = true,
                OpenGLESCompatibility = true,
                CGBitmapContextCompatibility = true,
                BytesPerRowAlignment = bytesPerRow,
                Height = frame.Height,
                Width = frame.Width
            };
            var pixelBuffer = CVPixelBuffer.Create(frame.Width, frame.Height, CVPixelFormatType.CV32BGRA, frame.Frame, bytesPerRow, attr);
            return pixelBuffer;
        }

        public static async Task<MLMultiArray> PlayMN(VNCoreMLModel _VNMLModel, CVPixelBuffer image)
        {
            var tcs = new TaskCompletionSource<MLMultiArray>();
            if (_VNMLModel == null)
            {
                tcs.TrySetCanceled();
            }

            var request = new VNCoreMLRequest(_VNMLModel, (response, e) =>
            {
                if (e != null)
                    tcs.SetException(new NSErrorException(e));
                else
                {
                    var results = response.GetResults<VNCoreMLFeatureValueObservation>();
                    var r = results.FirstOrDefault();
                    if (r != null)
                    {
                        var fv = r.FeatureValue.MultiArrayValue;
                        tcs.SetResult(fv);
                        r.FeatureValue.Dispose();
                    }
                    else
                    {
                        tcs.SetCanceled();
                    }

                }
            })
            {
                ImageCropAndScaleOption = VNImageCropAndScaleOption.ScaleFill
            };

            var requestHandler = new VNImageRequestHandler(image, new NSDictionary());

            requestHandler.Perform(new[] { request }, out NSError error);

            var classifications = await tcs.Task;

            if (error != null)
                throw new NSErrorException(error);

            return classifications;
        }
    }
}
