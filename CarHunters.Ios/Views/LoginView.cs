using System;
using CarHunters.Core.ViewModels;
using MvvmCross.Platforms.Ios.Views;
using CarHunters.Ios.Views.Abstract;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using MvvmCross.Binding.BindingContext;

namespace CarHunters.Ios.Views
{
    [MvxRootPresentation]
    public partial class LoginView : BaseView<LoginViewModel>
    {
        protected override void DoBind()
        {
            base.DoBind();

            var set = this.CreateBindingSet<LoginView, LoginViewModel>();
            set.Bind(_anonymousLogin).For("Tap").To(vm => vm.AnonymousLoginCommand);
            set.Bind(_facebookLogin).For("Tap").To(vm => vm.FacebookLoginCommand);
            set.Apply();
        }
    }
}