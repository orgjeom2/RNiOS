using Newtonsoft.Json;

namespace CarHunters.Core.Common.Extensions
{
	public static class CloneExtensions
	{
		public static T Clone<T>(this T source)
		{
		    return ReferenceEquals(source, null) ? default(T) : JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source));
		}
	}
}
