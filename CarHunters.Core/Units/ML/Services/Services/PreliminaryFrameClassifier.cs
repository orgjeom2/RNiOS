using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CarHunters.Core.PlatformAbstractions;
using CarHunters.Core.Units.ML.Services.Abstractions;
using MvvmCross;

namespace CarHunters.Core.Units.ML.Services.Services
{
    public class PreliminaryFrameClassifier : IPreliminaryFrameClassifier
    {
        //private readonly string INPUT_TENSOR_NAME = "input";
        //private readonly string OUT_TENSOR_NAME = "MobilenetV2/Predictions/Reshape_1";
        private readonly int OUT_SIZE = 1000;
        private readonly string LABELS_FILE_NAME = "imagenet_slim_labels.txt";
        private readonly List<string> VEHICLE_LABELS = new List<string>
        {
            "sports car",
            "limousine",
            "cab",
            "jeep",
            "garbage truck",
            "tow truck",
            "trailer truck",
            "minibus",
            "school bus",
            "minivan",
            "moving van",
            "police van",
            "golfcart",
            "pickup",
            "forklift",
            "snowmobile",
            "trolleybus",
            //"snowplow",
            //"harvester",
            //"streetcar",
            //"tractor",
            //"go-kart",
            "recreational vehicle"
        };

        //public event EventHandler<string> Elapsed;

        private IEntityAccessorService _accessor;
        private IImageClassifier _frameClassifier;

        public float VehicleThreashold() => 0.042f;

        public List<string> Labels { get; private set; }
        public List<float> Probabilities { get; private set; }

        public PreliminaryFrameClassifier(IEntityAccessorService accessor)
        {
            _accessor = accessor;
            _frameClassifier = accessor.Factory.CreateImageClassifier("mn_keras", OUT_SIZE);

            var file = accessor.Helpers.GetStreamByPath(LABELS_FILE_NAME);
            ReadLabels(file);
        }

        public async Task Classify(object image)
        {
            var probs = await _frameClassifier.Classify(image);
            Probabilities = probs.ToList();
        }

        public async Task<float> EstimateVehicleProbability(object image)
        {
            await Classify(image);

            float sumVehicleProb = 0;
            int i = 0;
            foreach (var label in Labels)
            {
                if (VEHICLE_LABELS.Contains(label))
                {
                    sumVehicleProb += Probabilities[i];
                }
                i++;
            }
            return sumVehicleProb;
        }

        private void ReadLabels(Stream stream)
        {
            try
            {
                string line;
                Labels = new List<string>();
                // Read the file and display it line by line.  
                using (StreamReader file = new StreamReader(stream))
                {
                    while ((line = file.ReadLine()) != null)
                    {
                        Labels.Add(line);
                    }

                    file.Close();
                }
            }
            catch (IOException e)
            {
                System.Diagnostics.Debug.WriteLine("+++INFO", "Could not read " + LABELS_FILE_NAME + ". " + e.Message);
            }
        }
    }
}