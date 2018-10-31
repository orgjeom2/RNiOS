using System;
using CarHunters.Core.Common.Models;

namespace CarHunters.Core.Common.Abstractions
{
    public interface INotificationHubService
    {
        event EventHandler<bool> OnConnectionChanged;
        event EventHandler<FrameEntry> OnNewFrame;
    }
}
