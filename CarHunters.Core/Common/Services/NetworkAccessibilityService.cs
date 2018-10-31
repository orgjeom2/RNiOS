using CarHunters.Core.Common.Abstractions;

namespace CarHunters.Core.Common.Services
{
    public class NetworkAccessibilityService : INetworkAccessibilityService
    {
        public bool HasAccess => Plugin.Connectivity.CrossConnectivity.Current.IsConnected;
    }
}
