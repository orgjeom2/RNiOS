using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CarHunters.Core.Common.Abstractions;
using CarHunters.Core.PlatformAbstractions;
using MvvmCross;
using MvvmCross.Logging;

namespace CarHunters.Core.Common.Services
{
    public class ExceptionHandlerService : IExceptionHandlerService
    {
		readonly IHockeyAppService _hockeyApp;

        readonly Dictionary<Type, string> _exceptionTypeDictionary = new Dictionary<Type, string>()
		{
			{typeof(TaskCanceledException), "The request timed out."},
		};

		public ExceptionHandlerService(IHockeyAppService hockeyAppService)
		{
			_hockeyApp = hockeyAppService;
		}

		public virtual void HandleException(Exception ex)
		{
		    var message = _exceptionTypeDictionary.ContainsKey(ex.GetType()) ? GetExceptionMessage(ex) : "Some error was occured";

			Mvx.IoCProvider.Resolve<IUserInteractionService>().Alert(message);
#if DEBUG
		    Mvx.IoCProvider.Resolve<IMvxLog>().Trace(message, message);
            //#else
		    _hockeyApp.LogError(ex);
#endif
		}

		public void HandleExceptionWithoutNotify(Exception ex)
		{

#if DEBUG
		    Mvx.IoCProvider.Resolve<IMvxLog>().Trace(ex.Message, ex.Message);
#else
            _hockeyApp.LogError(ex);
#endif
        }

		private string GetExceptionMessage(Exception ex)
		{
			string message;

			if (ex is TaskCanceledException exception)
			{
				//Android http client gets "task was cancaled" when timeout is happened
				message = exception.CancellationToken.IsCancellationRequested ? ex.Message : _exceptionTypeDictionary[exception.GetType()];
			}
			else
			{
				message = _exceptionTypeDictionary[ex.GetType()];
			}

			return message;
		}

	}
}
