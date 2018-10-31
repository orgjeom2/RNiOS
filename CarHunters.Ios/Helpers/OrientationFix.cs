using System;
using CoreGraphics;
using UIKit;

namespace CarHunters.Ios.Helpers
{
    public static class OrientationFix
    {
        public static UIImage FixOrientation(UIImage originalImage)
        {
            if (originalImage.Orientation == UIImageOrientation.Up)
                return originalImage;
            CGAffineTransform transform = CGAffineTransform.MakeIdentity();

            switch (originalImage.Orientation)
            {
                case UIImageOrientation.Down:
                case UIImageOrientation.DownMirrored:
                    transform.Rotate((float)Math.PI);
                    transform.Translate(originalImage.Size.Width, originalImage.Size.Height);
                    break;

                case UIImageOrientation.Left:
                case UIImageOrientation.LeftMirrored:
                    transform.Rotate((float)Math.PI / 2);
                    transform.Translate(originalImage.Size.Width, 0);
                    break;

                case UIImageOrientation.Right:
                case UIImageOrientation.RightMirrored:
                    transform.Rotate(-(float)Math.PI / 2);
                    transform.Translate(0, originalImage.Size.Height);
                    break;

                case UIImageOrientation.Up:
                case UIImageOrientation.UpMirrored:
                    break;
            }

            switch (originalImage.Orientation)
            {

                case UIImageOrientation.UpMirrored:
                case UIImageOrientation.DownMirrored:
                    transform.Translate(originalImage.Size.Width, 0);
                    transform.Scale(-1, 1);
                    break;

                case UIImageOrientation.LeftMirrored:
                case UIImageOrientation.RightMirrored:
                    transform.Translate(originalImage.Size.Height, 0);
                    transform.Scale(-1, 1);
                    break;

                case UIImageOrientation.Up:
                case UIImageOrientation.Down:
                case UIImageOrientation.Left:
                case UIImageOrientation.Right:
                    break;
            }

            var ctx = new CGBitmapContext(IntPtr.Zero, (nint)originalImage.Size.Width, (nint)originalImage.Size.Height, originalImage.CGImage.BitsPerComponent, originalImage.CGImage.BytesPerRow,
                       originalImage.CGImage.ColorSpace, originalImage.CGImage.BitmapInfo);

            ctx.ConcatCTM(transform);

            switch (originalImage.Orientation)
            {

                case UIImageOrientation.Left:
                case UIImageOrientation.LeftMirrored:
                case UIImageOrientation.Right:
                case UIImageOrientation.RightMirrored:

                    ctx.DrawImage(new CGRect(0, 0, originalImage.Size.Height, originalImage.Size.Width), originalImage.CGImage);
                    break;

                default:
                    ctx.DrawImage(new CGRect(0, 0, originalImage.Size.Width, originalImage.Size.Height), originalImage.CGImage);
                    break;
            }

            var cgImage = ctx.ToImage();

            UIImage result = UIImage.FromImage(cgImage);
            ctx.Dispose();
            cgImage.Dispose();

            return result;
        }
    }
}