using System;
using UIKit;
using CoreAnimation;
namespace CarHunters.Ios.Extensions
{
    public static class ImageExtensions
    {
        public static UIImage GetImageFromLayer(this CALayer layer)
        {
            UIGraphics.BeginImageContext(layer.Frame.Size);
            layer.RenderInContext(UIGraphics.GetCurrentContext());
            var image = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return image;
        }
    }
}
