using System.Threading.Tasks;

namespace CarHunters.Core.Units.ML.Services.Abstractions
{
    public interface IPreliminaryFrameClassifier
    {
        Task<float> EstimateVehicleProbability(object image);
        float VehicleThreashold();
    }
}