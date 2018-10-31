using Cirrious.FluentLayouts.Touch;
using Foundation;
using MvvmCross.Binding.BindingContext;
using CarHunters.Ios.Controllers;
using UIKit;
using CoreAnimation;
using CarHunters.Core.ViewModels.Abstract;
using CarHunters.Ios.Helpers;
using CarHunters.Ios.Extensions;

namespace CarHunters.Ios.Views.Abstract
{
    public abstract class BaseView<TViewModel> : MvxTextFieldResponderController<TViewModel> where TViewModel : BaseViewModel
    {
        UIActivityIndicatorView _activityView;

        protected BaseView() { }
        protected BaseView(string nibName, NSBundle bundle) : base(nibName, bundle) { }

        protected virtual UIColor TitleColor => UIColor.White;
        public virtual bool ShowActivityIndicator => false;

        public override UIStatusBarStyle PreferredStatusBarStyle()
        {
            return UIStatusBarStyle.LightContent;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = UIColor.White;

            if(NavigationController != null)
            {
                NavigationController.NavigationBar.BarStyle = UIBarStyle.Black;
                NavigationController.NavigationBar.TitleTextAttributes = new UIStringAttributes()
                {
                    ForegroundColor = TitleColor
                };
            }

            if (ShowActivityIndicator)
                AddIndicatorView();

            var set = this.CreateBindingSet<BaseView<TViewModel>, BaseViewModel>();
            set.Bind(this).For("IsBusy").To(vm => vm.IsBusy);
            set.Bind(this).For(v => v.Title).To(vm => vm.Title);
            set.Apply();

            InitView();
            DoViewDidLoad();
            ConstraintView();
            DoBind();
        }

        public bool IsBusy
        {
            set
            {
                UIApplication.SharedApplication.NetworkActivityIndicatorVisible = value;
            }
        }

        public override void DidMoveToParentViewController(UIViewController parent)
        {
            if (parent == null)
                Dispose(true);

            base.DidMoveToParentViewController(parent);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            SetNavigationButtonsEnabled(false);
            ViewModel.OnResume();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            SetNavigationButtonsEnabled(true);
        }

        void SetNavigationButtonsEnabled(bool enabled)
        {
            if (NavigationController?.NavigationBar != null)
                NavigationController.NavigationBar.UserInteractionEnabled = enabled;
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            if (IsViewLoaded)
                ViewModel.OnPause();
        }

        void AddIndicatorView()
        {
            _activityView = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.Gray)
            { 
                TranslatesAutoresizingMaskIntoConstraints = false,
                BackgroundColor = UIColor.Black.ColorWithAlpha(0.35f)
            };
            View.Add(_activityView);
            View.AddConstraints(
                _activityView.AtTopOf(View),
                _activityView.Leading().EqualTo().LeadingOf(View),
                _activityView.Trailing().EqualTo().TrailingOf(View),
                _activityView.AtBottomOf(View)
            );

            var set = this.CreateBindingSet<BaseView<TViewModel>, BaseViewModel>();
            set.Bind(this).For("ShowIndicator").To(vm => vm.IsBusy);
            set.Apply();
        }

        bool _showIndicator;
        public bool ShowIndicator
        {
            get { return _showIndicator; }
            set
            {
                _showIndicator = value;
                if (ShowActivityIndicator)
                {
                    if (ViewModel.IsBusy)
                    {
                        View.BringSubviewToFront(_activityView);
                        _activityView.Hidden = false;
                    }
                    else
                        _activityView.Hidden = true;
                }
            }
        }

        protected virtual void InitView() { }
        protected virtual void ConstraintView() { }
        protected virtual void DoViewDidLoad() { }
        protected virtual void DoBind() { }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ViewModel?.OnDestroy();
            }
            base.Dispose(disposing);
        }
    }

}
