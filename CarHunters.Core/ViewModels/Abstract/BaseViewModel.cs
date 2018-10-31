using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using CarHunters.Core.Common.Abstractions;
using CarHunters.Core.Common.Services;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;
using CarHunters.Core.PlatformAbstractions;

namespace CarHunters.Core.ViewModels.Abstract
{
    public abstract class BaseViewModel : MvxViewModel<ViewModelParameter, ViewModelResult>
	{
		protected readonly IMvxMessenger Messenger = Mvx.IoCProvider.Resolve<IMvxMessenger>();
	    protected readonly IMvxNavigationService NavigationService = Mvx.IoCProvider.Resolve<IMvxNavigationService>();
        protected readonly List<MvxSubscriptionToken> Subscriptions = new List<MvxSubscriptionToken>();
        protected readonly IServerCommandWrapperService ServerCommandWrapperService = new ServerCommandWrapperService();
        protected readonly IUserInteractionService UserInteractionService = Mvx.IoCProvider.Resolve<IUserInteractionService>();
        protected ViewModelParameter Parameter;

        public BaseViewModel()
        {
            ServerCommandWrapperService.IsBusyChanged += IsBusyChangedHandler;
        }

        #region Properties

        public bool NotIsBusy => !_isBusy;

        string _title = string.Empty;
        public virtual string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        bool _isBusy;
        public virtual bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (SetProperty(ref _isBusy, value))
                    RaisePropertyChanged(() => NotIsBusy);
            }
        }

        #endregion

        #region Methods

        public override void Prepare(ViewModelParameter parameter)
	    {
	        Parameter = parameter;
	    }

		public override void Start()
		{
			base.Start();
			DebugMethod();
		}

        public virtual void OnResume()
        {
            DebugMethod();
        }

        public virtual void OnPause()
        {
            DebugMethod();
        }

        void DebugMethod([System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
		{
			Debug.WriteLine("{1} of {0}", memberName, GetType().FullName);
		}

        void IsBusyChangedHandler(object sender, bool e) => IsBusy = e;

        #endregion

        public virtual void OnDestroy()
		{
			DebugMethod();
			foreach (var item in Subscriptions)
				item.Dispose();
        }

        MvxCommand _closeCommand;
		public virtual ICommand CloseCommand
		{
			get
			{
				return _closeCommand ?? (_closeCommand = new MvxCommand(async () => await NavigationService.Close(this)));
			}
		}
	}
}
