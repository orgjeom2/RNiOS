using System;
using System.Threading.Tasks;
using CarHunters.Core.Common.Models;

namespace CarHunters.Core.Units.Authorization.Services.Abstractions
{
    public interface IAuthorizationOperationService
    {
        Task<TryResult<string>> AnonymousLogin();
        Task<TryResult<string>> FacebookLogin();
    }
}
