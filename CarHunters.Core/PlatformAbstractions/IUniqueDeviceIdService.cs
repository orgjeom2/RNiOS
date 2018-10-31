using System;
namespace CarHunters.Core.PlatformAbstractions
{
    public interface IUniqueDeviceIdService
    {
        string UniqueDeviceId { get; }
    }
}
