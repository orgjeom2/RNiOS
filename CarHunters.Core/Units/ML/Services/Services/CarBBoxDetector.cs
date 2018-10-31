using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using CarHunters.Core.Units.ML.Services.Abstractions;
using CarHunters.Core.PlatformAbstractions;
using MvvmCross;
using CarHunters.Core.Common.Models;
using System.Drawing;

namespace CarHunters.Core.Units.ML.Services.Services
{
    public class CarBBoxDetector : ICarBBoxDetector
    {
        //private readonly string INPUT_TENSOR_NAME = "Preprocessor/sub";
        //private readonly string OUT_TENSOR_1 = "concat";
        //private readonly string OUT_TENSOR_2 = "concat_1";
        private readonly int OUT_BB_COUNT = 1917;
        private readonly string LABELS_FILE_NAME = "coco_labels_list.txt";
        private readonly List<string> VEHICLE_LABELS = new List<string>
        {
            "bus",
            "car",
            "truck",
            "motorbike"
        };
        private readonly float BBOX_PROB_THRESHOLD = 0.3f;
        private readonly float BBOX_MIN_SIZE = 0.1f;

        public event EventHandler<LogParams> LogChanged;

        public int InputSize() => 300;

        private IEntityAccessorService _accessor;
        private IMNSSD _mnssd;

        private float[] _mainBB;

        private List<string> Labels { get; set; }

        public CarBBoxDetector(IEntityAccessorService accessor)
        {
            _accessor = accessor;
            _mnssd = accessor.Factory.CreateMNSSD();

            var file = accessor.Helpers.GetStreamByPath(LABELS_FILE_NAME);
            ReadLabels(file);
        }

        private void ReadLabels(Stream stream)
        {
            try
            {
                string line;
                Labels = new List<string>();
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
                Debug.WriteLine("+++INFO", "Could not read " + LABELS_FILE_NAME + ". " + e.Message);
            }
        }

        public async Task<float[]> InferMainBBox(object image)
        {
            var res = await _mnssd.PlayMNSSD(image, Labels.Count, OUT_BB_COUNT);

            var startSelectBBTime = DateTimeOffset.UtcNow; 

            _mainBB = SelectMainBB(res.Item1, res.Item2);
            if (_mainBB[0] < -99) _mainBB = null;

            var selectBBTime = DateTimeOffset.UtcNow - startSelectBBTime;

            var preTime = _mnssd.PreprocessTime.TotalMilliseconds.ToString("F0");
            var inferTime = _mnssd.InferenceTime.TotalMilliseconds.ToString("F0");
            var postTime = _mnssd.PostprocessTime.TotalMilliseconds.ToString("F0");
            var logParams = new LogParams
            {
                ShowIdx = 2,
                Color = Color.LimeGreen,
                Text = $"MNSSD: {preTime}ms, {inferTime}ms, {postTime}ms{Environment.NewLine}" +
                       $" - Select mainBB: {selectBBTime.TotalMilliseconds:F0}ms"
            };
            LogChanged?.Invoke(this, logParams);

            return _mainBB;
        }

        private float[] SelectMainBB(float[] probsAll, float[] bboxesAll)
        {
            List<int> lstVehicleIds = new List<int>();
            for (int i = 0; i < Labels.Count; i++)
            {
                if (VEHICLE_LABELS.Contains(Labels[i]))
                    lstVehicleIds.Add(i);
            }
            int[] labelIds = lstVehicleIds.ToArray();
            float[] outBB = new float[4];
            NativeMethods.TranslateAndTakeMainBB(ref probsAll, ref bboxesAll, OUT_BB_COUNT,
                                                 Labels.Count, 1, ref labelIds, labelIds.Length,
                                                 BBOX_PROB_THRESHOLD, BBOX_MIN_SIZE, BBOX_MIN_SIZE,
                                                 ref outBB);
            return outBB;
        }
    }
}