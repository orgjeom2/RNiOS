using System;
using UIKit;
namespace CarHunters.Ios.Helpers
{
    public static class TopViewControllerHelper
    {
        public static UIViewController GetTopController()
        {
            var window = UIApplication.SharedApplication.KeyWindow;
            var controller = window.RootViewController;
            while (controller.PresentedViewController != null)
            {
                controller = controller.PresentedViewController;
            }

            return controller;
        }
    }
}
