using System;
using AVFoundation;
using UIKit;
using CoreGraphics;
using System.Drawing;

namespace CarHunters.Ios.Helpers
{
    public static class CameraHelper
    {
        public static AVCaptureVideoOrientation VideoOrientationFromCurrentDeviceOrientation()
        {
            switch (UIApplication.SharedApplication.StatusBarOrientation)
            {
                case UIInterfaceOrientation.Portrait:
                    return AVCaptureVideoOrientation.Portrait;
                case UIInterfaceOrientation.LandscapeLeft:
                    return AVCaptureVideoOrientation.LandscapeLeft;
                case UIInterfaceOrientation.LandscapeRight:
                    return AVCaptureVideoOrientation.LandscapeRight;
                case UIInterfaceOrientation.PortraitUpsideDown:
                    return AVCaptureVideoOrientation.PortraitUpsideDown;

            }
            return AVCaptureVideoOrientation.Portrait;
        }

        public static CGRect GetRectForBbox(RectangleF rectangle)
        {
            var screenBounds = UIScreen.MainScreen.Bounds;

            return new CGRect(rectangle.X * screenBounds.Width, 
                              rectangle.Y * screenBounds.Height, 
                              rectangle.Width * screenBounds.Width, 
                              rectangle.Height * screenBounds.Height);
        }
    }
}
