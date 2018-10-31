using Foundation;
using CarHunters.Ios.Helpers;
using UIKit;

namespace CarHunters.Ios.Extensions
{
    public static class ColorExtensions
	{
		public static UIColor ToUIColor(this int hexValue)
		{
			return UIColor.FromRGB(
				(((hexValue & 0xFF0000) >> 16) / 255.0f),
				(((hexValue & 0xFF00) >> 8) / 255.0f),
				((hexValue & 0xFF) / 255.0f)
			);
		}
	}
}
