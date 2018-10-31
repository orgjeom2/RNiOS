using System;
using System.Threading.Tasks;
using CarHunters.Core.Common.Models;

namespace CarHunters.Core.PlatformAbstractions
{
    public interface IFacebookLoginService
    {
        Task<TryResult<SocialData>> Login();
    }
}
