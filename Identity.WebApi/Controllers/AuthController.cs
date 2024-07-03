using Microsoft.AspNetCore.Mvc;
using Identity.WebApi.Models;
using Identity.WebApi.Services;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace Identity.WebApi.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;

		public AuthController(IAuthService authService)
		{
			_authService = authService;
		}

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

		[HttpPost]
		public async Task<IActionResult> Login([FromBody] LoginDto loginDto, CancellationToken cancellationToken)
		{
			try
			{
				var response = await _authService.LoginAsync(loginDto, cancellationToken);

				if (response == null)
				{
					return Unauthorized("Invalid login attempt.");
				}

				return Ok(response);
			}
			catch (InvalidOperationException ex)
			{
				if (ex.Message == "User account is locked.")
				{
					return Forbid("User account is locked.");
				}
				return StatusCode(500, "An error occurred during login.");
			}
		}

		[HttpPost("send-login-code")]
		public async Task<IActionResult> SendSmsLoginCode([FromBody] LoginDto loginRequestDto, CancellationToken cancellationToken)
		{
			var result = await _authService.SendSmsLoginCodeAsync(loginRequestDto, cancellationToken);
			if (!result)
			{
				return BadRequest("Invalid login attempt.");
			}

			return Ok("Verification code sent.");
		}

		[HttpPost("verify-login-code")]
		public async Task<IActionResult> VerifySmsLoginCode([FromBody] VerifyLoginCodeDto verifyCodeDto, CancellationToken cancellationToken)
		{
			var result = await _authService.VerifySmsLoginCodeAsync(verifyCodeDto, cancellationToken);
			if (result == null)
			{
				return BadRequest("Invalid verification code.");
			}

			return Ok(result);
		}

		[HttpPost("send-email-login-code")]
		public async Task<IActionResult> SendLoginCode([FromBody] EmailLoginRequestDto loginRequestDto, CancellationToken cancellationToken)
		{
			var result = await _authService.SendLoginCodeAsync(loginRequestDto, cancellationToken);

			if (!result)
			{
				return BadRequest("Invalid login attempt.");
			}

			return Ok("Verification code sent.");
		}

		[HttpPost("verify-email-login-code")]
		public async Task<IActionResult> VerifyLoginCode([FromBody] VerifyLoginCodeDto verifyCodeDto, CancellationToken cancellationToken)
		{
			var response = await _authService.VerifyLoginCodeAsync(verifyCodeDto, cancellationToken);

			if (response == null)
			{
				return Unauthorized("Invalid verification code.");
			}

			return Ok(response);
		}

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

		[HttpGet]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> GetAllUsersWithRoles(CancellationToken cancellationToken)
		{
			var usersWithRoles = await _authService.GetAllUsersWithRolesAsync(cancellationToken);
			return Ok(usersWithRoles);
		}

		[HttpPut]
		[Authorize]
		public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto updateProfileDto, CancellationToken cancellationToken)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (userId == null)
			{
				return Unauthorized("User is not logged in.");
			}

			var result = await _authService.UpdateProfileAsync(userId, updateProfileDto, cancellationToken);
			if (!result.Succeeded)
			{
				return BadRequest(result.Errors);
			}

			return Ok("Profile updated successfully.");
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> ExternalLogin([FromQuery] string provider, [FromQuery] string returnUrl = null)
		{
			var properties = await _authService.ExternalLoginAsync(provider, returnUrl);
			return Challenge(properties, provider);
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> ExternalLoginCallback()
		{
			var response = await _authService.ExternalLoginCallbackAsync();
			if (response == null)
			{
				return BadRequest("External login failed.");
			}

			return Ok(response);
		}
	}
}
