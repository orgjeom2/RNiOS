using System.Collections.Generic;
using System.Threading.Tasks;
using Plugin.Permissions.Abstractions;

namespace CarHunters.Core.Common.Abstractions
{
    public interface IPermissionsService
    {
		IPermissions Instance { get; }
		Task<bool> PreCheckPermissionsAccesGrantedAsync(Permission permission);
        Task<PermissionStatus> PreCheckPermissionsAccessAsync(Permission permission);
		Task<bool> CheckPermissionsAccesGrantedAsync(Permission permission);
		Task<bool> CheckPermissionsAccesGrantedAsync(List<Permission> permissions);
        bool OpenSettings();
    }
}
