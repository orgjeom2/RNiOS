using CarHunters.Core.ViewModels.Abstract;
using MvvmCross.Commands;
using CarHunters.Core.Resources;
using CarHunters.Core.Units.Authorization.Services.Abstractions;

namespace CarHunters.Core.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public override string Title => Translator.GetText("LoginString");

        private readonly IAuthorizationOperationService _authorizationOperationService;

        public LoginViewModel(IAuthorizationOperationService authorizationOperationService)
        {
            _authorizationOperationService = authorizationOperationService;
        }

        private MvxAsyncCommand _anonymousLoginCommand;
        public IMvxAsyncCommand AnonymousLoginCommand
        {
            get
            {
                return
                    _anonymousLoginCommand ?? (_anonymousLoginCommand = new MvxAsyncCommand(async () =>
                    {
                        await ServerCommandWrapperService.InternetServerCommandWrapperWithBusy(async () =>
                        {
                            var loginResult = await _authorizationOperationService.AnonymousLogin();
                            if (!loginResult.OperationSucceeded)
                            {
                                await UserInteractionService.AlertAsync(loginResult.Result);
                                return;
                            }

                            await NavigationService.Navigate<LastCheckinsViewModel>();
                        });
                    }));
            }
        }

        private MvxAsyncCommand _facebookLoginCommand;
        public IMvxAsyncCommand FacebookLoginCommand
        {
            get
            {
                return
                    _facebookLoginCommand ?? (_facebookLoginCommand = new MvxAsyncCommand(async () =>
                    {
                        await ServerCommandWrapperService.InternetServerCommandWrapperWithBusy(async () =>
                        {
                            var loginResult = await _authorizationOperationService.FacebookLogin();
                            if (!loginResult.OperationSucceeded)
                                return;

                            await NavigationService.Navigate<LastCheckinsViewModel>();
                        });
                    }));
            }
        }
    }
}