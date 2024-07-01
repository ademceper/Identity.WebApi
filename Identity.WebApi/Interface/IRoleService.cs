using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Identity.WebApi.Services
{
	public interface IRoleService
	{
		Task<IdentityResult> CreateRoleAsync(RoleDto roleDto, CancellationToken cancellationToken);
		Task<IdentityResult> DeleteRoleAsync(string roleId, CancellationToken cancellationToken);
		Task<IdentityResult> AssignRoleToUserAsync(UserRoleDto userRoleDto, CancellationToken cancellationToken);
		Task<IdentityResult> RemoveRoleFromUserAsync(UserRoleDto userRoleDto, CancellationToken cancellationToken);
		Task<IList<string>> GetUserRolesAsync(string userId, CancellationToken cancellationToken);
		Task<IList<string>> GetRolesAsync(CancellationToken cancellationToken);
	}
}
