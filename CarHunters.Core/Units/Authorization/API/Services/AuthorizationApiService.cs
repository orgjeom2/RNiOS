using System;
using CarHunters.Core.Common.Services;
using CarHunters.Core.PlatformAbstractions;
using CarHunters.Core.Units.Network.Services.Abstractions;
using System.Threading.Tasks;
using CarHunters.Core.Units.Authorization.API.Abstractions;
using UserService;
using Google.Protobuf;
using Grpc.Core;
using CarHunters.Core.Common.Enums;
using CarHunters.Core.Resources;
using CarHunters.Core.Common.Models;

namespace CarHunters.Core.Units.Authorization.API.Services
{
    public class AuthorizationApiService : BaseApiService, IAuthorizationApiService
    {
        private readonly IUniqueDeviceIdService uniqueDeviceIdService;
        private readonly IFacebookLoginService facebookLoginService;
        private readonly ITimelessTokenService timelessTokenService;

        private readonly UserAuth.UserAuthClient _client;

        public AuthorizationApiService(IUniqueDeviceIdService uniqueDeviceIdService,
                                       IFacebookLoginService facebookLoginService,
                                       ITimelessTokenService timelessTokenService)
        {
            this.uniqueDeviceIdService = uniqueDeviceIdService;
            this.facebookLoginService = facebookLoginService;
            this.timelessTokenService = timelessTokenService;

            _client = new UserAuth.UserAuthClient(ConnectionChannel);
        }

        public async Task<TryResult<string>> AnonymousLogin()
        {
            var loginInfo = new LoginInfo
            {
                Anon = new LoginInfo.Types.AnonymousInfo
                {
                    DeviceId = ByteString.CopyFromUtf8(uniqueDeviceIdService.UniqueDeviceId)
                }
            };

            var loginResult = await _client.loginAsync(loginInfo, new CallOptions());
            if (loginResult?.Error == null)
            {
                this.timelessTokenService.AuthorizationToken = loginResult.AuthToken;
                return TryResult.Create(true, string.Empty);
            }

            return TryResult.Create(false, loginResult?.Error?.Message);
        }

        public async Task<TryResult<string>> FacebookLogin()
        {
            var result = await facebookLoginService.Login();
            if (!result.OperationSucceeded)
            {
                if (result.Result?.LoginState == SocialLoginStateEnum.Canceled)
                    return TryResult.Create(false, Translator.GetText("FacebookLoginCanceled"));
                if (result.Result?.LoginState == SocialLoginStateEnum.Failed)
                    return TryResult.Create(false, result.Result?.ErrorString);
            }

            var loginInfo = new LoginInfo
            {
                Fb = new LoginInfo.Types.FacebookInfo
                {
                    Token = result.Result.Token
                }
            };

            var loginResult = await _client.loginAsync(loginInfo, new CallOptions());
            if (loginResult?.Error != null)
            {
                return TryResult.Create(false, loginResult?.Error?.Message);
            }

            this.timelessTokenService.AuthorizationToken = loginResult.AuthToken;
            return TryResult.Create(true, string.Empty);
        }
    }
}
