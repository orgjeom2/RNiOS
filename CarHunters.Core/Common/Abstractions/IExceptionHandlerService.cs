using System;

namespace CarHunters.Core.Common.Abstractions
{
    public interface IExceptionHandlerService
    {
		void HandleException(Exception ex);
		void HandleExceptionWithoutNotify(Exception ex);
    }
}
