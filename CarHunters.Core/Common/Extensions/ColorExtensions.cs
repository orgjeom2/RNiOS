using System;
using CarHunters.Core.Common.Models;
using System.Drawing;
namespace CarHunters.Core.Common.Extensions
{
    public static class ColorExtensions
    {
        public static CustomColor ToOurColor(this Color color)
        {
            return new CustomColor
            {
                Red = color.R,
                Green = color.G,
                Blue = color.B,
                Alpha = color.A
            };
        }
    }
}
