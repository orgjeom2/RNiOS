using System;

namespace CarHunters.Core.Helpers
{
	internal static class DateConvertor
	{
        internal static long GetCurrentTimeInUtc => (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;

	    internal static DateTime UnixTimeStampToDateTime(this long unixTimeStamp)
		{
			var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

			return dateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
		}

	    public static long DateTimeInUtcMilliseconds(this DateTime dateTime)
	    {
	        return (long)dateTime.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
	    }
    }
}
