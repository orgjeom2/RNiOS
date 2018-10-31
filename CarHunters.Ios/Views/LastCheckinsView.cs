using CarHunters.Ios.Views.Abstract;
using CarHunters.Core.ViewModels;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Binding.BindingContext;

namespace CarHunters.Ios.Views
{
    [MvxRootPresentation(WrapInNavigationController = true)]
    public partial class LastCheckinsView : BaseView<LastCheckinsViewModel>
    {
        protected override void DoBind()
        {
            base.DoBind();

            var set = this.CreateBindingSet<LastCheckinsView, LastCheckinsViewModel>();
            //set.Bind(_bottomView.HuntButton).To(vm => vm.OpenHuntModeCommand);
            set.Bind(_huntButton).To(vm => vm.OpenHuntModeCommand);
            set.Apply();
        }
    }
}

