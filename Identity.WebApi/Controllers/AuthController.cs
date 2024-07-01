using Microsoft.AspNetCore.Mvc;
using Identity.WebApi.Models;
using Identity.WebApi.Services;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Identity.WebApi.Controllers
{
	/// <summary>
	/// Controller for handling authentication and authorization.
	/// </summary>
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;

		/// <summary>
		/// Initializes a new instance of the <see cref="AuthController"/> class.
		/// </summary>
		/// <param name="authService">The authentication service.</param>
		public AuthController(IAuthService authService)
		{
			_authService = authService;
		}

		/// <summary>
		/// Registers a new user.
		/// </summary>
		/// <param name="registerDto">The registration DTO containing user details.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>An IActionResult representing the result of the registration process.</returns>
		[HttpPost]
		public async Task<IActionResult> Register([FromBody] RegisterDto registerDto, CancellationToken cancellationToken)
		{
			var result = await _authService.RegisterAsync(registerDto, cancellationToken);

			if (!result.Succeeded)
			{
				return BadRequest(result.Errors);
			}

			return Ok("User registered successfully.");
		}

		[HttpGet]
		//[Authorize(Roles = "Admin")]
		public async Task<IActionResult> GetAllUsersWithRoles(CancellationToken cancellationToken)
		{
			var usersWithRoles = await _authService.GetAllUsersWithRolesAsync(cancellationToken);
			return Ok(usersWithRoles);
		}

		/// <summary>
		/// Logs in a user.
		/// </summary>
		/// <param name="loginDto">The login DTO containing user credentials.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>An IActionResult containing the login response DTO.</returns>
		[HttpPost]
		public async Task<IActionResult> Login([FromBody] LoginDto loginDto, CancellationToken cancellationToken)
		{
			var response = await _authService.LoginAsync(loginDto, cancellationToken);

			if (response == null)
			{
				return Unauthorized("Invalid login attempt.");
			}

			return Ok(response);
		}

		/// <summary>
		/// Changes the password of the logged-in user.
		/// </summary>
		/// <param name="changePasswordDto">The DTO containing the current and new passwords.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>An IActionResult representing the result of the password change.</returns>
		[HttpPost]
		[Authorize]
		public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto, CancellationToken cancellationToken)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId == null)
			{
				return Unauthorized("User is not logged in.");
			}

			var result = await _authService.ChangePasswordAsync(userId, changePasswordDto, cancellationToken);

			if (!result.Succeeded)
			{
				return BadRequest(result.Errors);
			}

			return Ok("Password changed successfully.");
		}

		/// <summary>
		/// Initiates the password reset process by sending a reset code to the user's email.
		/// </summary>
		/// <param name="forgotPasswordRequestDto">The DTO containing the user's email.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>An IActionResult representing the result of the password reset request.</returns>
		[HttpPost]
		public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto forgotPasswordRequestDto, CancellationToken cancellationToken)
		{
			var result = await _authService.ForgotPasswordAsync(forgotPasswordRequestDto, cancellationToken);

			if (!result.Succeeded)
			{
				return BadRequest(result.Errors);
			}

			return Ok("Password reset code sent.");
		}

		/// <summary>
		/// Resets the user's password using the provided reset code.
		/// </summary>
		/// <param name="resetPasswordDto">The DTO containing the email, reset code, and new password.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>An IActionResult representing the result of the password reset.</returns>
		[HttpPost]
		public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto, CancellationToken cancellationToken)
		{
			var result = await _authService.ResetPasswordAsync(resetPasswordDto, cancellationToken);

			if (!result.Succeeded)
			{
				return BadRequest(result.Errors);
			}

			return Ok("Password reset successfully.");
		}
	}
}
