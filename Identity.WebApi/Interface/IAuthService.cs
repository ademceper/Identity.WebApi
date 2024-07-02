using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Identity.WebApi.Models;

namespace Identity.WebApi.Services
{
	public interface IAuthService
	{
		Task<IdentityResult> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken);
		Task<LoginResponseDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken);
		Task<IdentityResult> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto, CancellationToken cancellationToken);
		Task<IdentityResult> ForgotPasswordAsync(ForgotPasswordRequestDto forgotPasswordRequestDto, CancellationToken cancellationToken);
		Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto, CancellationToken cancellationToken);
		Task<bool> SendLoginCodeAsync(UsernameLoginRequestDto loginRequestDto, CancellationToken cancellationToken);
		Task<LoginResponseDto> VerifyLoginCodeAsync(VerifyLoginCodeDto verifyCodeDto, CancellationToken cancellationToken);
		Task<List<UserWithRolesDto>> GetAllUsersWithRolesAsync(CancellationToken cancellationToken);
		Task<IdentityResult> UpdateProfileAsync(string userId, UpdateProfileDto updateProfileDto, CancellationToken cancellationToken);
		Task<AuthenticationProperties> ExternalLoginAsync(string provider, string returnUrl = null);
		Task<LoginResponseDto> ExternalLoginCallbackAsync();
	}
}
