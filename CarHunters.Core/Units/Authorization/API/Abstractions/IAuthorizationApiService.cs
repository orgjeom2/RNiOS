using System;
using System.Threading.Tasks;
using CarHunters.Core.Common.Models;

namespace CarHunters.Core.Units.Authorization.API.Abstractions
{
    public interface IAuthorizationApiService
    {
        Task<TryResult<string>> AnonymousLogin();
        Task<TryResult<string>> FacebookLogin();
    }
}
