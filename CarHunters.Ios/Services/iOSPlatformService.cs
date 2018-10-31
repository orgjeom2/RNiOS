using System;
using System.Globalization;
using CarHunters.Core.PlatformAbstractions;
using Foundation;
using UIKit;

namespace OCBB.Ios.Services
{
    public class iOSPlatformService : IPlatformService
    {
        readonly CultureInfo _preferredculture;

        public iOSPlatformService()
        {
            _preferredculture = new CultureInfo(NSLocale.PreferredLanguages[0].Substring(0,2));
        }

        public CultureInfo GetPreferredCulture() => _preferredculture;

        public void OpenUrlLink(string url)
        {
            if (string.IsNullOrEmpty(url))
                return;
			
			url = url.Trim();
			if (!url.StartsWith("http", StringComparison.Ordinal))
				url = "http://" + url;

            UIApplication.SharedApplication.OpenUrl(new NSUrl(url));
        }

        public bool IsAppInForeground
        {
            get
            {
                bool isAppInForeground = true;
                UIApplication.SharedApplication.InvokeOnMainThread(() =>
                {
                    isAppInForeground = UIApplication.SharedApplication.ApplicationState == UIApplicationState.Active;
                });
                return isAppInForeground;
            }
        }

		public bool IsAppRunning
		{
			get
			{
				throw new NotImplementedException();
			}
		}

        public void HideApp()
        {
            throw new NotImplementedException();
        }
    }
}
