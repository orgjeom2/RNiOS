using System;
using System.Threading.Tasks;
using CarHunters.Core.Common.Abstractions;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;

namespace CarHunters.Core.Common.Services
{
	internal class ConnectivityService : IConnectivityService
	{
		public event EventHandler<ConnectivityChangedEventArgs> Connected;
		public ConnectivityService ()
		{
			CrossConnectivity.Current.ConnectivityChanged += ConnectivityChanged;
		}

		public bool IsConnected => CrossConnectivity.Current.IsConnected;

		public async void ConnectivityChanged (object sender, ConnectivityChangedEventArgs e)
		{
            if (e.IsConnected)
                await CheckReachableWithRetry();

            Connected?.Invoke(this, e);
		}

        private async Task CheckReachableWithRetry()
        { 
            var result = await CheckReachable();
            if (!result)
            {
                await Task.Delay(50);
                await CheckReachable();
            }
        }

        private async Task<bool> CheckReachable()
        {
            try
            {
                return await CrossConnectivity.Current.IsRemoteReachable("google.com");
            }
            catch (Exception)
            {
                return false;
            }
        }
	}
}
