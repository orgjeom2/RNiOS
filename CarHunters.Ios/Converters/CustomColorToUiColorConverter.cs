using System;
using MvvmCross.Converters;
using CarHunters.Core.Common.Models;
using UIKit;
using System.Globalization;

namespace CarHunters.Ios.Converters
{
    public class CustomColorToUiColorConverter : MvxValueConverter<CustomColor, UIColor>
    {
        protected override UIColor Convert(CustomColor value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return UIColor.Clear;

            return UIColor.FromRGBA(value.Red, value.Green, value.Blue, value.Alpha);
        }
    }
}
