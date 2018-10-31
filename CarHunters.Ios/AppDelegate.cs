using CarHunters.Core;
using Facebook.CoreKit;
using Foundation;
using MvvmCross.Platforms.Ios.Core;
using UIKit;
using MvvmCross;
using MvvmCross.Plugin.Messenger;
using CarHunters.Core.Common.Models.Messages;
using System;
using CarHunters.Core.PlatformAbstractions;

namespace CarHunters.Ios
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : MvxApplicationDelegate<Setup, App>
    {
        // class-level declarations

        public override UIWindow Window
        {
            get;
            set;
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            var result = base.FinishedLaunching(application, launchOptions);

            ApplicationDelegate.SharedInstance.FinishedLaunching(application, launchOptions);
            // Override point for customization after application launch.
            // If not required for your application you can safely delete this method

            return result;
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            var handled = ApplicationDelegate.SharedInstance.OpenUrl(app, url, options);

            return handled;
        }

        public override void OnResignActivation(UIApplication application)
        {
            // Invoked when the application is about to move from active to inactive state.
            // This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message)
            // or when the user quits the application and it begins the transition to the background state.
            // Games should use this method to pause the game.

            try
            {
                Mvx.IoCProvider.Resolve<IMvxMessenger>().Publish(new ChangeStateAppMessage(this, false));
            }
            catch(Exception ex)
            {
                Mvx.IoCProvider.Resolve<IHockeyAppService>().LogError(ex);
            }
        }

        public override void DidEnterBackground(UIApplication application)
        {
            // Use this method to release shared resources, save user data, invalidate timers and store the application state.
            // If your application supports background exection this method is called instead of WillTerminate when the user quits.

            try
            {
                Mvx.IoCProvider.Resolve<IMvxMessenger>().Publish(new ChangeStateAppMessage(this, true));
            }
            catch (Exception ex)
            {
                Mvx.IoCProvider.Resolve<IHockeyAppService>().LogError(ex);
            }
        }

        public override void WillEnterForeground(UIApplication application)
        {
            // Called as part of the transiton from background to active state.
            // Here you can undo many of the changes made on entering the background.

            try
            {
                Mvx.IoCProvider.Resolve<IMvxMessenger>().Publish(new ChangeStateAppMessage(this, false));
            }
            catch (Exception ex)
            {
                Mvx.IoCProvider.Resolve<IHockeyAppService>().LogError(ex);
            }
        }

        public override void OnActivated(UIApplication application)
        {
            // Restart any tasks that were paused (or not yet started) while the application was inactive.
            // If the application was previously in the background, optionally refresh the user interface.
        }

        public override void WillTerminate(UIApplication application)
        {
            // Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
        }
    }
}

