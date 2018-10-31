using System;
using CarHunters.Core.Common.Abstractions;
using CarHunters.Core.Common.Models;

namespace CarHunters.Core.Common.Services
{
    internal class NotificationHubService : INotificationHubService, IInternalNotificationHubService
    {
        public event EventHandler<bool> OnConnectionChanged;
        public event EventHandler<FrameEntry> OnNewFrame;

        public void ConnectionChanged(bool isConnectd)
        {
            OnConnectionChanged?.Invoke(this, isConnectd);
        }

        public void NewFrame(FrameEntry frameEntry)
        {
            OnNewFrame?.Invoke(this, frameEntry);
        }
    }
}