using System.Runtime.CompilerServices;

namespace CarHunters.Core.PlatformAbstractions
{
	public interface ISettingsBaseService
	{
		void Set<T>(T value, [CallerMemberName] string key = "");
		T Get<T>(T defaultValue = default(T), [CallerMemberName] string key = "");
	}
}
