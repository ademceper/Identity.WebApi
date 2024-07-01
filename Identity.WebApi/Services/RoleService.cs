using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Identity.WebApi.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace Identity.WebApi.Services
{
	public class RoleService : IRoleService
	{
		private readonly RoleManager<IdentityRole<Guid>> _roleManager;
		private readonly UserManager<AppUser> _userManager;

		public RoleService(RoleManager<IdentityRole<Guid>> roleManager, UserManager<AppUser> userManager)
		{
			_roleManager = roleManager;
			_userManager = userManager;
		}

		public async Task<IdentityResult> CreateRoleAsync(RoleDto roleDto, CancellationToken cancellationToken)
		{
			var role = new IdentityRole<Guid> { Name = roleDto.Name };
			return await _roleManager.CreateAsync(role);
		}

		public async Task<IdentityResult> DeleteRoleAsync(string roleId, CancellationToken cancellationToken)
		{
			var role = await _roleManager.FindByIdAsync(roleId);
			if (role == null)
			{
				return IdentityResult.Failed(new IdentityError { Description = "Role not found." });
			}
			return await _roleManager.DeleteAsync(role);
		}

		public async Task<IdentityResult> AssignRoleToUserAsync(UserRoleDto userRoleDto, CancellationToken cancellationToken)
		{
			var user = await _userManager.FindByIdAsync(userRoleDto.UserId);
			if (user == null)
			{
				return IdentityResult.Failed(new IdentityError { Description = "User not found." });
			}

			var role = await _roleManager.FindByIdAsync(userRoleDto.RoleId);
			if (role == null)
			{
				return IdentityResult.Failed(new IdentityError { Description = "Role not found." });
			}

			return await _userManager.AddToRoleAsync(user, role.Name);
		}

		public async Task<IdentityResult> RemoveRoleFromUserAsync(UserRoleDto userRoleDto, CancellationToken cancellationToken)
		{
			var user = await _userManager.FindByIdAsync(userRoleDto.UserId);
			if (user == null)
			{
				return IdentityResult.Failed(new IdentityError { Description = "User not found." });
			}

			var role = await _roleManager.FindByIdAsync(userRoleDto.RoleId);
			if (role == null)
			{
				return IdentityResult.Failed(new IdentityError { Description = "Role not found." });
			}

			return await _userManager.RemoveFromRoleAsync(user, role.Name);
		}

		public async Task<IList<string>> GetUserRolesAsync(string userId, CancellationToken cancellationToken)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return null;
			}
			return await _userManager.GetRolesAsync(user);
		}

		public async Task<IList<string>> GetRolesAsync(CancellationToken cancellationToken)
		{
			return await _roleManager.Roles.Select(r => r.Name).ToListAsync(cancellationToken);
		}
	}
}
