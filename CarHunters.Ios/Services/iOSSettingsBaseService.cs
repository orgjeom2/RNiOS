using System.Runtime.CompilerServices;
using CarHunters.Core.PlatformAbstractions;
using Foundation;
using Newtonsoft.Json;

namespace CarHunters.Ios.Services
{
	public class iOSSettingsBaseService : ISettingsBaseService
	{
		private const string NULL = "!NULL";
		private readonly NSUserDefaults _preferences;

		public iOSSettingsBaseService()
		{
			_preferences = NSUserDefaults.StandardUserDefaults;
		}

		public void Set<T>(T value, [CallerMemberName] string key = "")
		{
			var str = JsonConvert.SerializeObject(value);
			_preferences.SetString(str ?? NULL, key);
		}

		public T Get<T>(T defaultValue = default(T), [CallerMemberName] string key = "")
		{
			var str = _preferences.StringForKey(key);
			if (str == NULL || string.IsNullOrEmpty(str))
			{
				return defaultValue;
			}
			var obj = JsonConvert.DeserializeObject<T>(str);
			return obj;
		}
	}
}
