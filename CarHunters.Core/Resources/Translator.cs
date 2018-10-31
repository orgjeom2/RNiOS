using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using CarHunters.Core.PlatformAbstractions;
using MvvmCross;
using MvvmCross.Logging;

namespace CarHunters.Core.Resources
{
	public static class Translator
	{
		static readonly ResourceManager ResManager;
		static readonly CultureInfo CultureInfo;

		static Translator()
		{
			var assembly = typeof(Translator).GetTypeInfo().Assembly;
			ResManager = new ResourceManager("СarHunters.Core.Resources.Resources", assembly);

            CultureInfo = Mvx.IoCProvider.Resolve<IPlatformService>().GetPreferredCulture();
		}

		public static string GetText(string key = null, params object[] parameters)
		{
			try
			{				
				var text = ResManager.GetString(key, CultureInfo)?.Replace("\\n", "\n");
			    if (string.IsNullOrEmpty(text))
			        return key;

				return parameters != null && parameters.Any() ? string.Format(text, parameters) : text;
			}
			catch
			{
			    Mvx.IoCProvider.Resolve<IMvxLog>().Trace("Resource not found for {0}", key);
			}

			return key;
		}

	    public static string GetLanguage()
	    {
	        var cultureName = CultureInfo.Name;

	        if (cultureName == "ru")
	            return "ru";

	        //TODO: When other language ready
	        //if (cultureName == "en-US" || cultureName == "en-GB")
	        //return "ua";

	        return "uk";
	    }
    }
}
