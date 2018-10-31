using System.Threading.Tasks;
using CarHunters.Core.ViewModels;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using MvvmCross;
using CarHunters.Core.Units.Network.Services.Abstractions;

namespace CarHunters.Core
{
    public class AppStart: MvxAppStart
    {
	    private readonly IMvxNavigationService _mvxNavigationService;

        public AppStart(IMvxNavigationService mvxNavigationService, IMvxApplication application) : base(application,
            mvxNavigationService)
        {
            _mvxNavigationService = mvxNavigationService;
        }

        protected override async Task NavigateToFirstViewModel(object hint = null)
        {
            var authorizationToken = Mvx.IoCProvider.Resolve<ITimelessTokenService>().AuthorizationToken;

            if(string.IsNullOrEmpty(authorizationToken))
            {
                await _mvxNavigationService.Navigate<LoginViewModel>();
                return;
            }

            await _mvxNavigationService.Navigate<LastCheckinsViewModel>();
        }
    }
}
