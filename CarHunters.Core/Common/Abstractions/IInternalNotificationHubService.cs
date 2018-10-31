using CarHunters.Core.Common.Models;
namespace CarHunters.Core.Common.Abstractions
{
    public interface IInternalNotificationHubService
    {
        void ConnectionChanged(bool isConnectd);
        void NewFrame(FrameEntry frameEntry);
    }
}