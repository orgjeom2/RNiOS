using System.Globalization;

namespace CarHunters.Core.PlatformAbstractions
{
    public interface IPlatformService
	{
		CultureInfo GetPreferredCulture();
		void OpenUrlLink(string url);

		bool IsAppInForeground { get; }
		bool IsAppRunning { get; }
		
	    void HideApp();
	}
}
