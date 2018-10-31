using System;

namespace CarHunters.Core.Common.Models
{
    public class CarFeatures
    {
        public string ModelName { get; set; }
        public string ModelDbId { get; set; }
        public float ModelProbability { get; set; }

        public string ColorName { get; set; }
        public string ColorDbId { get; set; }
        public float ColorProbability { get; set; }
    }
}