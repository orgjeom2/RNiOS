using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CarHunters.Core.Common.Models;
using Facebook.LoginKit;
using CarHunters.Core.PlatformAbstractions;
using Facebook.CoreKit;
using Foundation;
using CarHunters.Core.Common.Enums;
using UIKit;
using CarHunters.Ios.Helpers;
using MvvmCross.Platforms.Ios;

namespace CarHunters.Ios.Services
{
    public class iOSFacebookLoginService : IFacebookLoginService
    {
        List<string> readPermissions = new List<string> { "public_profile", "email" };
        readonly LoginManager _loginManager = new LoginManager();
        TaskCompletionSource<TryResult<SocialData>> _facebookTask;
        LoginButton _loginView;
        SocialData _socialData;

        public Task<TryResult<SocialData>> Login()
        {
            _facebookTask = new TaskCompletionSource<TryResult<SocialData>>();
            _loginManager.LogInWithReadPermissions(readPermissions.ToArray(),
                                                   TopViewControllerHelper.GetTopController(),
                                                   LoginHandler);
            return _facebookTask.Task;
        }

        void LoginHandler(LoginManagerLoginResult loginResult, NSError error)
        {
            if (loginResult.IsCancelled)
                _facebookTask.TrySetResult(TryResult.Create(false, 
                                                            new SocialData 
            { 
                LoginState = SocialLoginStateEnum.Canceled 
            }));
            else if (error != null)
            {
                _facebookTask.TrySetResult(TryResult.Create(false, 
                                                            new SocialData 
                { 
                    LoginState = SocialLoginStateEnum.Failed, 
                    ErrorString = error.LocalizedDescription 
                }));
            }
            else
            {
                _socialData = new SocialData
                {
                    Token = loginResult.Token.TokenString,
                    UserId = loginResult.Token.UserId,
                    ExpireAt = loginResult.Token.ExpirationDate.ToDateTimeUtc()
                };
                _facebookTask.TrySetResult(TryResult.Create(true, _socialData));
            }
        }
    }
}
