using System;
using CarHunters.Core.PlatformAbstractions;
using HockeyApp.iOS;

namespace OCBB.Ios.Services
{
    public class iOSHockeyAppService : IHockeyAppService
    {
		BITHockeyManager _hockeyAppManager;
		string _hockeyAppAPIKey = "put your key";

		public iOSHockeyAppService()
		{
			Initialize();
		}

		void Initialize()
		{
			_hockeyAppManager = BITHockeyManager.SharedHockeyManager;
			_hockeyAppManager.Configure(_hockeyAppAPIKey);
			_hockeyAppManager.StartManager();
#if !DEBUG
            _hockeyAppManager.Authenticator.AuthenticateInstallation();
#endif
		}

		public void LogError(Exception e)
		{
#if !DEBUG
            _hockeyAppManager.MetricsManager.TrackEvent($"Handle exception {e.Message} {e.StackTrace}");
#endif
		}
    }
}
