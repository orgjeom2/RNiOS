using System;
using System.Threading.Tasks;
using CarHunters.Core.Common.Abstractions;
using CarHunters.Core.PlatformAbstractions;
using CarHunters.Core.Resources;
using MvvmCross;

namespace CarHunters.Core.Common.Services
{
    public class ServerCommandWrapperService : IServerCommandWrapperService
    {
		public event EventHandler<bool> IsBusyChanged;

		bool _isBusy;
		public bool IsBusy
		{
			get => _isBusy;
		    private set
			{
				_isBusy = value;
				IsBusyChanged?.Invoke(this, value);
			}
		}

		public async Task InternetServerCommandWrapperWithBusy(Func<Task> action)
		{
			if (!Mvx.IoCProvider.Resolve<INetworkAccessibilityService>().HasAccess)
			{
			    Mvx.IoCProvider.Resolve<IUserInteractionService>().Alert(Translator.GetText("ConnectionRequiredString"));
				return;
			}
			try
			{
				IsBusy = true;
				await action();
			}
			catch (Exception ex)
			{
			    Mvx.IoCProvider.Resolve<IExceptionHandlerService>().HandleException(ex);
			}
			finally
			{
				IsBusy = false;
			}
		}

		public async Task ServerCommandWrapper(Func<Task> action)
		{
			if (IsBusy)
			{
				return;
			}
			try
			{
				IsBusy = true;
				await action();
			}
			catch (Exception ex)
			{
			    Mvx.IoCProvider.Resolve<IExceptionHandlerService>().HandleException(ex);
			}
			finally
			{
				IsBusy = false;
			}
		}

		public async Task ServerCommandWrapperParallel(Func<Task> action)
		{
			try
			{
				await action();
			}
			catch (Exception ex)
			{
			    Mvx.IoCProvider.Resolve<IExceptionHandlerService>().HandleException(ex);
			}
		}
	}
}
