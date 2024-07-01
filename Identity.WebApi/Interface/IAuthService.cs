using Identity.WebApi.Models;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Identity.WebApi.Services
{
	/// <summary>
	/// Defines the contract for authentication and authorization services.
	/// </summary>
	public interface IAuthService
	{
		Task<List<UserWithRolesDto>> GetAllUsersWithRolesAsync(CancellationToken cancellationToken);

		/// <summary>
		/// Registers a new user.
		/// </summary>
		/// <param name="registerDto">The registration DTO containing user details.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>IdentityResult representing the result of the registration process.</returns>
		Task<IdentityResult> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken);

		/// <summary>
		/// Logs in a user.
		/// </summary>
		/// <param name="loginDto">The login DTO containing user credentials.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>LoginResponseDto containing the user details and JWT token.</returns>
		Task<LoginResponseDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken);

		/// <summary>
		/// Changes the password of a user.
		/// </summary>
		/// <param name="userId">The ID of the user whose password is to be changed.</param>
		/// <param name="changePasswordDto">The DTO containing the current and new passwords.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>IdentityResult representing the result of the password change.</returns>
		Task<IdentityResult> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto, CancellationToken cancellationToken);

		/// <summary>
		/// Initiates the password reset process by sending a reset code to the user's email.
		/// </summary>
		/// <param name="forgotPasswordRequestDto">The DTO containing the user's email.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>IdentityResult representing the result of the password reset request.</returns>
		Task<IdentityResult> ForgotPasswordAsync(ForgotPasswordRequestDto forgotPasswordRequestDto, CancellationToken cancellationToken);

		/// <summary>
		/// Resets the user's password using the provided reset code.
		/// </summary>
		/// <param name="resetPasswordDto">The DTO containing the email, reset code, and new password.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>IdentityResult representing the result of the password reset.</returns>
		Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto, CancellationToken cancellationToken);
	}
}
