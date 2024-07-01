using Microsoft.AspNetCore.Mvc;
using Identity.WebApi.Models;
using Identity.WebApi.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Identity.WebApi.Controllers
{
	/// <summary>
	/// Controller for managing roles.
	/// </summary>
	[Route("api/[controller]/[action]")]
	[ApiController]
	//[Authorize(Roles = "Admin")] // Only users with Admin role can access the methods in this controller
	public class RoleController : ControllerBase
	{
		private readonly IRoleService _roleService;

		/// <summary>
		/// Initializes a new instance of the <see cref="RoleController"/> class.
		/// </summary>
		/// <param name="roleService">The role service.</param>
		public RoleController(IRoleService roleService)
		{
			_roleService = roleService;
		}

		/// <summary>
		/// Creates a new role.
		/// </summary>
		/// <param name="roleDto">The DTO containing role information.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>An IActionResult representing the result of the role creation.</returns>
		[HttpPost]
		public async Task<IActionResult> CreateRole([FromBody] RoleDto roleDto, CancellationToken cancellationToken)
		{
			var result = await _roleService.CreateRoleAsync(roleDto, cancellationToken);
			if (!result.Succeeded)
			{
				return BadRequest(result.Errors);
			}
			return Ok("Role created successfully.");
		}

		/// <summary>
		/// Deletes an existing role.
		/// </summary>
		/// <param name="roleId">The ID of the role to delete.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>An IActionResult representing the result of the role deletion.</returns>
		[HttpDelete]
		public async Task<IActionResult> DeleteRole([FromBody] string roleId, CancellationToken cancellationToken)
		{
			var result = await _roleService.DeleteRoleAsync(roleId, cancellationToken);
			if (!result.Succeeded)
			{
				return BadRequest(result.Errors);
			}
			return Ok("Role deleted successfully.");
		}

		/// <summary>
		/// Assigns a role to a user.
		/// </summary>
		/// <param name="userRoleDto">The DTO containing user and role information.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>An IActionResult representing the result of the role assignment.</returns>
		[HttpPost]
		public async Task<IActionResult> AssignRoleToUser([FromBody] UserRoleDto userRoleDto, CancellationToken cancellationToken)
		{
			var result = await _roleService.AssignRoleToUserAsync(userRoleDto, cancellationToken);
			if (!result.Succeeded)
			{
				return BadRequest(result.Errors);
			}
			return Ok("Role assigned to user successfully.");
		}

		/// <summary>
		/// Removes a role from a user.
		/// </summary>
		/// <param name="userRoleDto">The DTO containing user and role information.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>An IActionResult representing the result of the role removal.</returns>
		[HttpPost]
		public async Task<IActionResult> RemoveRoleFromUser([FromBody] UserRoleDto userRoleDto, CancellationToken cancellationToken)
		{
			var result = await _roleService.RemoveRoleFromUserAsync(userRoleDto, cancellationToken);
			if (!result.Succeeded)
			{
				return BadRequest(result.Errors);
			}
			return Ok("Role removed from user successfully.");
		}

		/// <summary>
		/// Gets the roles assigned to a user.
		/// </summary>
		/// <param name="userId">The user ID.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>An IActionResult containing a list of role names assigned to the user.</returns>
		[HttpGet]
		public async Task<IActionResult> GetUserRoles([FromQuery] string userId, CancellationToken cancellationToken)
		{
			var roles = await _roleService.GetUserRolesAsync(userId, cancellationToken);
			if (roles == null)
			{
				return NotFound("User not found.");
			}
			return Ok(roles);
		}

		/// <summary>
		/// Gets all available roles.
		/// </summary>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>An IActionResult containing a list of role names.</returns>
		[HttpGet]
		public async Task<IActionResult> GetRoles(CancellationToken cancellationToken)
		{
			var roles = await _roleService.GetRolesAsync(cancellationToken);
			return Ok(roles);
		}
	}
}
