using System.Collections.Generic;
//using Xamarin.Essentials;
using CarHunters.Core.Common.Models;

namespace CarHunters.Core.Common.Models
{
    public class IdentifiedCarParams
    {
        public List<CarFeatures> TopResults { get; set; }
        public FrameEntry CroppedCarImage { get; set; }
        //public Location GeoLocation { get; set; }
    }
}
