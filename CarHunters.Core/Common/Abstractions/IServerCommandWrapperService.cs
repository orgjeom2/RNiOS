using System;
using System.Threading.Tasks;

namespace CarHunters.Core.Common.Abstractions
{
    public interface IServerCommandWrapperService
    {
        event EventHandler<bool> IsBusyChanged;
        Task InternetServerCommandWrapperWithBusy(Func<Task> action);
        Task ServerCommandWrapper(Func<Task> action);
        Task ServerCommandWrapperParallel(Func<Task> action); 
    }
}
