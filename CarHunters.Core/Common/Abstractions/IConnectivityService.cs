using System;
using Plugin.Connectivity.Abstractions;

namespace CarHunters.Core.Common.Abstractions
{
	public interface IConnectivityService
	{
		bool IsConnected { get; }
		event EventHandler<ConnectivityChangedEventArgs> Connected;
	}
}
