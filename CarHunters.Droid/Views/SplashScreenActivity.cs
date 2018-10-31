using Android.App;
using Android.Content.PM;
using Android.OS;
using MvvmCross.Platforms.Android.Views;

namespace CarHunters.Droid.Views
{
    [Activity(MainLauncher = true, Theme = "@style/AppTheme.NoActionBar", ScreenOrientation = ScreenOrientation.Portrait, NoHistory = true)]
    public class SplashScreenActivity : MvxSplashScreenActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
    }
}