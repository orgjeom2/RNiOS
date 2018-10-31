using System;
using CarHunters.Core.ViewModels.Abstract;
using CarHunters.Core.Resources;
using MvvmCross.Commands;
namespace CarHunters.Core.ViewModels
{
    public class LastCheckinsViewModel : BaseViewModel
    {
        public override string Title => Translator.GetText("LastCheckins");

        private MvxCommand _openHuntModeCommand;
        public IMvxCommand OpenHuntModeCommand
        {
            get
            {
                return _openHuntModeCommand ?? (_openHuntModeCommand = new MvxCommand(async () =>
                {
                    await NavigationService.Navigate<HuntViewModel>();
                }));
            }
        }
    }
}
