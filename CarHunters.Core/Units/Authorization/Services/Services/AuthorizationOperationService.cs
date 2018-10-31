using CarHunters.Core.Units.Authorization.Services.Abstractions;
using System.Threading.Tasks;
using CarHunters.Core.Common.Models;
using CarHunters.Core.Units.Authorization.API.Abstractions;

namespace CarHunters.Core.Units.Authorization.Services.Services
{
    public partial class AuthorizationOperationService : IAuthorizationOperationService
    {
        private readonly IAuthorizationApiService authorizationApiService;

        public AuthorizationOperationService(IAuthorizationApiService authorizationApiService)
        {
            this.authorizationApiService = authorizationApiService;
        }

        public Task<TryResult<string>> AnonymousLogin()
        {
            return authorizationApiService.AnonymousLogin();
        }

        public Task<TryResult<string>> FacebookLogin()
        {
            return authorizationApiService.FacebookLogin();
        }
    }
}
