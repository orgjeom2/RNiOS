using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CarHunters.Core.Common.Models;
using CarHunters.Core.PlatformAbstractions;
using CarHunters.Core.Units.ML.Services.Abstractions;

namespace CarHunters.Core.Units.ML.Services.Services
{
    public class CarColorClassifier : ICarFeatureClassifier
    {
        //private static readonly string INPUT_TENSOR_NAME = "input_1";
        //private static readonly string OUT_TENSOR_NAME = "dense_1/Softmax";
        private static readonly string LABELS_FILE_NAME = "color_net_labels.txt";
        private static readonly int IN_SIZE = 256;
        private static readonly int OUT_SIZE = 16;

        private IEntityAccessorService _accessor;
        private IImageClassifier _colorClassifier;

        public int InputSize() => IN_SIZE;
        public int OutputSize() => OUT_SIZE;

        public TimeSpan PreprocessTime => _colorClassifier.PreprocessTime;
        public TimeSpan InferenceTime => _colorClassifier.InferenceTime;
        public TimeSpan PostprocessTime => _colorClassifier.PostprocessTime;

        public float BestProbability => Probabilities.Max();
        public string BestLabel => Labels[BestLabelId];

        public List<string> Labels { get; private set; }
        public List<string> LabelDbIds { get; private set; }
        public List<float> Probabilities { get; private set; }

        public int BestLabelId
        {
            get
            {
                float p = Probabilities.Max();
                return Probabilities.IndexOf(p);
            }
        }

        public CarColorClassifier(IEntityAccessorService accessor)
        {
            _accessor = accessor;
            _colorClassifier = accessor.Factory.CreateImageClassifier("colors", OUT_SIZE);

            var file = accessor.Helpers.GetStreamByPath(LABELS_FILE_NAME);
            ReadLabels(file);
        }

        public async Task Classify(object image)
        {
            var probs = await _colorClassifier.Classify(image);
            Probabilities = probs.ToList();
        }

        private void ReadLabels(Stream stream)
        {
            try
            {
                string line;
                Labels = new List<string>();
                LabelDbIds = new List<string>();
                // Read the file and display it line by line.  
                using (StreamReader file = new StreamReader(stream))
                {
                    while ((line = file.ReadLine()) != null)
                    {
                        var entry = line.Split(new char[] { '\t' });
                        Labels.Add(entry[1]);
                        LabelDbIds.Add(entry[0]);
                    }
                    file.Close();
                }
            }
            catch (IOException e)
            {
                System.Diagnostics.Debug.WriteLine("+++INFO", "Could not read " + LABELS_FILE_NAME + ". " + e.Message);
            }
        }

    } // end of class definition
}