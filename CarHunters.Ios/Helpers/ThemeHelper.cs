using CarHunters.Ios.Extensions;
using UIKit;
using CoreAnimation;
using Foundation;
using CoreGraphics;

namespace CarHunters.Ios.Helpers
{
    public static class ThemeHelper
    {
        public static void SetInnerNavigationControllerStyle(this UINavigationController navigation, bool withLargeTitles = true)
        {
            if (navigation == null)
                return;

            navigation.NavigationBar.TintColor = UIColor.White;
            navigation.NavigationItem.RightBarButtonItem = new UIBarButtonItem(string.Empty, UIBarButtonItemStyle.Plain, null);

            var gradient = new CAGradientLayer
            {
                Frame = navigation.NavigationBar.Bounds,
                Colors = new[] 
                { 
                    Theme.GradientStartBlueColor.ToUIColor().CGColor, 
                    Theme.GradientEndGreenColor.ToUIColor().CGColor
                },
                StartPoint = new CGPoint(0.0, 0.5),
                EndPoint = new CGPoint(1.0, 0.5f)
            };
            navigation.NavigationBar.SetBackgroundImage(gradient.GetImageFromLayer(), UIBarMetrics.Default);
        }
    }
}
