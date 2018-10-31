using System.Drawing;
using System.IO;
using CarHunters.Core.Common.Models;
using CarHunters.Core.PlatformAbstractions;
using CarHunters.Core.Units.ML.Services.Services;

using UIKit;
using Foundation;
using CoreGraphics;
using VideoToolbox;

namespace CarHunters.Ios.Helpers
{
    public class iOSImageHelpers : IImageHelpers
    {
        public string AppVersion()
        {
            var version = NSBundle.MainBundle.InfoDictionary["CFBundleVersion"].ToString();
            return $"iOS {version}";
        }

        public byte[] ToJpegBytes(FrameEntry frame)
        {
            var cvBuffer = NNHelpers.ToCVPixelBuffer(frame);
            cvBuffer.ToCGImage(out CGImage image);
            cvBuffer.Dispose();
            return UIImage.FromImage(image).AsJPEG().ToArray();
        }

        public object ToNativeImage(FrameEntry frame)
        {
            return NNHelpers.ToCVPixelBuffer(frame);
        }

        // (!) @path will be treated as a filename
        public Stream GetStreamByPath(string path)
        {
            string[] arr = path.Split('.');
            NSUrl assetPath = NSBundle.MainBundle.GetUrlForResource(arr[0], arr[1]);
            return new FileStream(assetPath.Path, FileMode.Open, FileAccess.Read);
        }

        public int DeviceOrientationAsNumber()
        {
            var o = UIDevice.CurrentDevice.Orientation;
            if (o == UIDeviceOrientation.Portrait) return 0;
            if (o == UIDeviceOrientation.LandscapeLeft) return 1;
            if (o == UIDeviceOrientation.PortraitUpsideDown) return 2;
            if (o == UIDeviceOrientation.LandscapeRight) return 3;
            return 0;
        }

        public RectangleF RotateBBoxClockwise(RectangleF bb, int rotateTimes)
        {
            rotateTimes = rotateTimes & 3;
            if (rotateTimes <= 0) return bb;
            RectangleF rotated = RectangleF.FromLTRB(1 - bb.Bottom, bb.Left, 1 - bb.Top, bb.Right);
            return RotateBBoxClockwise(rotated, rotateTimes - 1);
        }

        public FrameEntry RotateImage(FrameEntry frame, int rotateTimes)
        {
            byte[] rotatedBytes = new byte[frame.Frame.Length];
            NativeMethods.RotateImage(frame.Frame, frame.Width, frame.Height, 4,
                                      rotateTimes, rotatedBytes);
            return new FrameEntry()
            {
                Frame = rotatedBytes,
                Width = rotateTimes % 2 == 0 ? frame.Width : frame.Height,
                Height = rotateTimes % 2 == 0 ? frame.Height : frame.Width
            };
        }

        public FrameEntry CropImage(FrameEntry frame, Rectangle rect)
        {
            int bytesPerPixel = 4;
            byte[] croppedBytes = new byte[rect.Width * rect.Height * bytesPerPixel];
            NativeMethods.CropImage(frame.Frame, frame.Width, frame.Height, bytesPerPixel,
                                    rect.Left, rect.Top, rect.Right, rect.Bottom, croppedBytes);
            FrameEntry croppedFrame = new FrameEntry()
            {
                Frame = croppedBytes,
                Width = rect.Width,
                Height = rect.Height
            };
            return croppedFrame;
        }
    }
}
