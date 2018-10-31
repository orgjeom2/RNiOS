using System;
using System.Linq;
using System.Drawing;
using System.Threading.Tasks;
using System.Collections.Generic;
using CarHunters.Core.Units.ML.Services.Abstractions;
using CarHunters.Core.Common.Models;
using CarHunters.Core.PlatformAbstractions;

namespace CarHunters.Core.Units.ML.Services.Services
{
    public class TopCarFeaturesDetector : ITopCarFeaturesDetector
    {
        private IEntityAccessorService _accessor;
        private float[] _carModelProbSum;
        private float[] _carColorProbSum;
        private int _feedCount;

        public FrameEntry LastCarImage { get; private set; }

        public event EventHandler<LogParams> LogChanged;

        public TopCarFeaturesDetector(IEntityAccessorService accessor)
        {
            _accessor = accessor;
            _carModelProbSum = new float[accessor.CarModelClassifier.OutputSize()];
            _carColorProbSum = new float[accessor.CarColorClassifier.OutputSize()];
        }

        private Rectangle ScaleCarBBoxAddingMargins(FrameEntry frame, RectangleF carBB)
        {
            int widthMargin = (int)(carBB.Width * frame.Width * 0.05);
            int heightMargin = (int)(carBB.Height * frame.Height * 0.05);
            int left = Math.Max(0, (int)(carBB.Left * frame.Width - widthMargin));
            int top = Math.Max(0, (int)(carBB.Top * frame.Height - heightMargin));
            int right = Math.Min(frame.Width, (int)(carBB.Right * frame.Width + widthMargin));
            int bottom = Math.Min(frame.Height, (int)(carBB.Bottom * frame.Height + heightMargin));
            return Rectangle.FromLTRB(left, top, right, bottom);
        }

        public async Task FeedFrame(FrameEntry frame, RectangleF carBB, bool isFirst)
        {
            Rectangle bb = ScaleCarBBoxAddingMargins(frame, carBB);
            var croppedCar = _accessor.Helpers.CropImage(frame, bb);

            var orientNumber = _accessor.Helpers.DeviceOrientationAsNumber();
            croppedCar = _accessor.Helpers.RotateImage(croppedCar, orientNumber);

            LastCarImage = croppedCar;
            // _accessor.DumpService.SaveFrame(croppedCar);

            var carImage = _accessor.Helpers.ToNativeImage(croppedCar);

            await _accessor.CarModelClassifier.Classify(carImage);
            await _accessor.CarColorClassifier.Classify(carImage);

            if (carImage is IDisposable disposable)
            {
                disposable.Dispose();
            }

            if (isFirst)
            {
                //Array.Fill(_carModelProbSum, 0.0f);
                //Array.Fill(_carColorProbSum, 0.0f);
                for (int i = 0; i < _carModelProbSum.Length; i++)
                    _carModelProbSum[i] = 0;
                for (int i = 0; i < _carColorProbSum.Length; i++)
                    _carColorProbSum[i] = 0;
                _feedCount = 0;
            }

            _feedCount++;

            for (int idx = 0; idx < _accessor.CarModelClassifier.OutputSize(); idx++)
                _carModelProbSum[idx] += _accessor.CarModelClassifier.Probabilities[idx];

            for (int idx = 0; idx < _accessor.CarColorClassifier.OutputSize(); idx++)
                _carColorProbSum[idx] += _accessor.CarColorClassifier.Probabilities[idx];

            // ----------------------------------------- //
            // Throw events for the screen log

            var top3 = TopKFeatures(3);

            var colorPreTime = _accessor.CarColorClassifier.PreprocessTime.TotalMilliseconds.ToString("F0");
            var colorInferTime = _accessor.CarColorClassifier.InferenceTime.TotalMilliseconds.ToString("F0");
            var colorPostTime = _accessor.CarColorClassifier.PostprocessTime.TotalMilliseconds.ToString("F0");
            var logText = $"Color NN: {colorPreTime}ms, {colorInferTime}ms, {colorPostTime}ms{Environment.NewLine}" +
                          $" - {top3[0].ColorName} - {100 * top3[0].ColorProbability:F1}%{Environment.NewLine}" +
                          $" - {top3[1].ColorName} - {100 * top3[1].ColorProbability:F1}%{Environment.NewLine}" +
                          $" - {top3[2].ColorName} - {100 * top3[2].ColorProbability:F1}%{Environment.NewLine}";
            var mmPreTime = _accessor.CarModelClassifier.PreprocessTime.TotalMilliseconds.ToString("F0");
            var mmInferTime = _accessor.CarModelClassifier.InferenceTime.TotalMilliseconds.ToString("F0");
            var mmPostTime = _accessor.CarModelClassifier.PostprocessTime.TotalMilliseconds.ToString("F0");
            logText += $"M/m NN: {mmPreTime}ms, {mmInferTime}ms, {mmPostTime}ms{Environment.NewLine}" +
                       $" - {top3[0].ModelName} - {100 * top3[0].ModelProbability:F1}%{Environment.NewLine}" +
                       $" - {top3[1].ModelName} - {100 * top3[1].ModelProbability:F1}%{Environment.NewLine}" +
                       $" - {top3[2].ModelName} - {100 * top3[2].ModelProbability:F1}%{Environment.NewLine}";
            var logParams = new LogParams()
            {
                ShowIdx = 3,
                Color = Color.Cyan,
                Text = logText
            };
            LogChanged?.Invoke(this, logParams);

            // ----------------------------------------- //
        }

        public List<CarFeatures> TopKFeatures(int K)
        {
            var modelClassifier = _accessor.CarModelClassifier;
            var models = new List<Tuple<string, string, float>>();
            for (int idx = 0; idx < modelClassifier.Labels.Count; idx++)
            {
                models.Add(Tuple.Create(modelClassifier.LabelDbIds[idx],
                                        modelClassifier.Labels[idx],
                                        _carModelProbSum[idx] / _feedCount));
            }
            models = models.OrderByDescending(x => x.Item3).ToList();

            var colorClassifier = _accessor.CarColorClassifier;
            var colors = new List<Tuple<string, string, float>>();
            for (int idx = 0; idx < colorClassifier.Labels.Count; idx++)
            {
                colors.Add(Tuple.Create(colorClassifier.LabelDbIds[idx],
                                        colorClassifier.Labels[idx],
                                        _carColorProbSum[idx] / _feedCount));
            }
            colors = colors.OrderByDescending(x => x.Item3).ToList();

            var ret = new List<CarFeatures>();
            for (int idx = 0; idx < K; idx++)
            {
                ret.Add(new CarFeatures()
                {
                    ModelDbId = models[idx].Item1,
                    ModelName = models[idx].Item2,
                    ModelProbability = models[idx].Item3,

                    ColorDbId = colors[idx].Item1,
                    ColorName = colors[idx].Item2,
                    ColorProbability = colors[idx].Item3
                });
            }
            return ret;
        }
    }
}