using System;
using System.Globalization;
using MvvmCross.Converters;
namespace CarHunters.Core.Common.Converters
{
    public class BoolInversionConverter : MvxValueConverter<bool, bool>
    {
        protected override bool Convert(bool value, Type targetType, object parameter, CultureInfo culture)
        {
            return !value;
        }
    }
}
