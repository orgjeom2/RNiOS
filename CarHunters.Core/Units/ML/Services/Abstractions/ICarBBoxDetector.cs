using System;
using System.Threading.Tasks;
using CarHunters.Core.Common.Models;

namespace CarHunters.Core.Units.ML.Services.Abstractions
{
    public interface ICarBBoxDetector
    {
        Task<float[]> InferMainBBox(object image);
        event EventHandler<LogParams> LogChanged;
    }
}