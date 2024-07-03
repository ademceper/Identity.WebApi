using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Identity.WebApi.Models;
using Identity.WebApi.Helpers;
using Identity.WebApi.Context;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Identity.WebApi.Services
{
	public class AuthService : IAuthService
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;
		private readonly IMapper _mapper;
		private readonly IJwtHelper _jwtHelper;
		private readonly IEmailService _emailService;
		private readonly ISmsService _smsService;
		private readonly AppDbContext _context;
		private readonly IConfiguration _configuration;
		private readonly ILogger<AuthService> _logger;

		public AuthService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IMapper mapper, IJwtHelper jwtHelper, IEmailService emailService, ISmsService smsService, AppDbContext context, IConfiguration configuration, ILogger<AuthService> logger)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_mapper = mapper;
			_jwtHelper = jwtHelper;
			_emailService = emailService;
			_smsService = smsService;
			_context = context;
			_configuration = configuration;
			_logger = logger;
		}

		public async Task<IdentityResult> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken)
		{
			if (registerDto.Password != registerDto.ConfirmPassword)
			{
				return IdentityResult.Failed(new IdentityError { Description = "Passwords do not match." });
			}

			var existingUserByEmail = await _userManager.FindByEmailAsync(registerDto.Email);
			if (existingUserByEmail != null)
			{
				return IdentityResult.Failed(new IdentityError { Description = "Email is already taken." });
			}

			var user = _mapper.Map<AppUser>(registerDto);

			var result = await _userManager.CreateAsync(user, registerDto.Password);

			return result;
		}

		public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken)
		{
			var user = await _userManager.FindByEmailAsync(loginDto.Email);
			if (user == null)
			{
				_logger.LogWarning("User not found for email: {Email}", loginDto.Email);
				throw new InvalidOperationException("Invalid email or password.");
			}

			if (await _userManager.IsLockedOutAsync(user))
			{
				_logger.LogWarning("User account is locked for email: {Email}", loginDto.Email);
				throw new InvalidOperationException("User account is locked.");
			}

			var result = await _signInManager.PasswordSignInAsync(user.UserName, loginDto.Password, isPersistent: false, lockoutOnFailure: true);
			if (!result.Succeeded)
			{
				_logger.LogWarning("Invalid email or password for email: {Email}", loginDto.Email);
				throw new InvalidOperationException("Invalid email or password.");
			}

			var roles = await _userManager.GetRolesAsync(user);
			var token = _jwtHelper.GenerateJwtToken(user, roles);

			var response = _mapper.Map<LoginResponseDto>(user);
			response.Token = token;
			response.Roles = roles.ToList();

			return response;
		}

		public async Task<IdentityResult> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto, CancellationToken cancellationToken)
		{
			if (changePasswordDto.NewPassword != changePasswordDto.ConfirmNewPassword)
			{
				return IdentityResult.Failed(new IdentityError { Description = "New passwords do not match." });
			}

			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return IdentityResult.Failed(new IdentityError { Description = "User not found." });
			}

			var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);

			if (result.Succeeded)
			{
				user.PasswordLastChanged = DateTime.UtcNow;
				await _userManager.UpdateAsync(user);
			}

			return result;
		}

		public async Task<IdentityResult> ForgotPasswordAsync(ForgotPasswordRequestDto forgotPasswordRequestDto, CancellationToken cancellationToken)
		{
			var user = await _userManager.FindByEmailAsync(forgotPasswordRequestDto.Email);
			if (user == null)
			{
				return IdentityResult.Failed(new IdentityError { Description = "User not found." });
			}

			var code = GenerateVerificationCode();
			var expiryTime = DateTime.UtcNow.AddMinutes(3);
			var passwordResetCode = new PasswordResetCode
			{
				Email = forgotPasswordRequestDto.Email,
				Code = code,
				CreatedAt = DateTime.UtcNow,
				ExpiryTime = expiryTime // Kodun geçerlilik süresi 3 dakika
			};

			_context.PasswordResetCodes.Add(passwordResetCode);
			await _context.SaveChangesAsync(cancellationToken);

			await _emailService.SendEmailAsync(forgotPasswordRequestDto.Email, "Password Reset Code", $"Your password reset code is: {code}", expiryTime.AddHours(3));

			return IdentityResult.Success;
		}

		public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto, CancellationToken cancellationToken)
		{
			var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
			if (user == null)
			{
				return IdentityResult.Failed(new IdentityError { Description = "User not found." });
			}

			var storedCode = await _context.PasswordResetCodes.FirstOrDefaultAsync(c => c.Email == resetPasswordDto.Email && c.Code == resetPasswordDto.Code && c.ExpiryTime > DateTime.UtcNow, cancellationToken);
			if (storedCode == null)
			{
				return IdentityResult.Failed(new IdentityError { Description = "Invalid or expired code." });
			}

			if (resetPasswordDto.NewPassword != resetPasswordDto.ConfirmPassword)
			{
				return IdentityResult.Failed(new IdentityError { Description = "Passwords do not match." });
			}

			var token = await _userManager.GeneratePasswordResetTokenAsync(user);
			var result = await _userManager.ResetPasswordAsync(user, token, resetPasswordDto.NewPassword);

			if (result.Succeeded)
			{
				_context.PasswordResetCodes.Remove(storedCode);
				await _context.SaveChangesAsync(cancellationToken);
			}

			return result;
		}

		public async Task<bool> SendSmsLoginCodeAsync(LoginDto loginRequestDto, CancellationToken cancellationToken)
		{
			var user = await _userManager.FindByEmailAsync(loginRequestDto.Email);
			if (user == null)
			{
				return false;
			}

			var result = await _signInManager.CheckPasswordSignInAsync(user, loginRequestDto.Password, lockoutOnFailure: false);
			if (!result.Succeeded)
			{
				return false;
			}

			var code = GenerateVerificationCode();
			var expiryTime = DateTime.UtcNow.AddMinutes(3);
			var verificationCode = new VerificationCode
			{
				Email = user.Email,
				Code = code,
				CreatedAt = DateTime.UtcNow,
				ExpiryTime = expiryTime // Kodun geçerlilik süresi 3 dakika
			};

			_context.VerificationCodes.Add(verificationCode);
			await _context.SaveChangesAsync(cancellationToken);

			await _smsService.SendSmsAsync(user.PhoneNumber, $"Your login verification code is: {code}. It will expire at {expiryTime.AddHours(3):HH:mm} GMT.");

			return true;
		}

		public async Task<LoginResponseDto> VerifySmsLoginCodeAsync(VerifyLoginCodeDto verifyCodeDto, CancellationToken cancellationToken)
		{
			var user = await _userManager.FindByEmailAsync(verifyCodeDto.Email);
			if (user == null)
			{
				return null;
			}

			var result = await _signInManager.CheckPasswordSignInAsync(user, verifyCodeDto.Password, lockoutOnFailure: false);
			if (!result.Succeeded)
			{
				return null;
			}

			var verificationCode = await _context.VerificationCodes
				.FirstOrDefaultAsync(vc => vc.Email == user.Email && vc.Code == verifyCodeDto.Code && vc.ExpiryTime > DateTime.UtcNow, cancellationToken);

			if (verificationCode == null)
			{
				return null;
			}

			var roles = await _userManager.GetRolesAsync(user);
			var token = _jwtHelper.GenerateJwtToken(user, roles);

			var response = _mapper.Map<LoginResponseDto>(user);
			response.Token = token;

			_context.VerificationCodes.Remove(verificationCode);
			await _context.SaveChangesAsync(cancellationToken);

			return response;
		}

		public async Task<bool> SendLoginCodeAsync(EmailLoginRequestDto loginRequestDto, CancellationToken cancellationToken)
		{
			var user = await _userManager.FindByEmailAsync(loginRequestDto.Email);
			if (user == null)
			{
				return false;
			}

			var result = await _signInManager.CheckPasswordSignInAsync(user, loginRequestDto.Password, lockoutOnFailure: false);
			if (!result.Succeeded)
			{
				return false;
			}

			var code = GenerateVerificationCode();
			var expiryTime = DateTime.UtcNow.AddMinutes(3);
			var verificationCode = new VerificationCode
			{
				Email = user.Email,
				Code = code,
				CreatedAt = DateTime.UtcNow,
				ExpiryTime = expiryTime // Kodun geçerlilik süresi 3 dakika
			};

			_context.VerificationCodes.Add(verificationCode);
			await _context.SaveChangesAsync(cancellationToken);

			await _emailService.SendEmailAsync(user.Email, "Login Verification Code", $"Your login verification code is: {code}", expiryTime.AddHours(3));

			return true;
		}

		public async Task<LoginResponseDto> VerifyLoginCodeAsync(VerifyLoginCodeDto verifyCodeDto, CancellationToken cancellationToken)
		{
			var user = await _userManager.FindByEmailAsync(verifyCodeDto.Email);
			if (user == null)
			{
				return null;
			}

			var result = await _signInManager.CheckPasswordSignInAsync(user, verifyCodeDto.Password, lockoutOnFailure: false);
			if (!result.Succeeded)
			{
				return null;
			}

			var verificationCode = await _context.VerificationCodes
				.FirstOrDefaultAsync(vc => vc.Email == user.Email && vc.Code == verifyCodeDto.Code && vc.ExpiryTime > DateTime.UtcNow, cancellationToken);

			if (verificationCode == null)
			{
				return null;
			}

			var roles = await _userManager.GetRolesAsync(user);
			var token = _jwtHelper.GenerateJwtToken(user, roles);

			var response = _mapper.Map<LoginResponseDto>(user);
			response.Token = token;
			response.Roles = roles.ToList();

			_context.VerificationCodes.Remove(verificationCode);
			await _context.SaveChangesAsync(cancellationToken);

			return response;
		}

		private string GenerateVerificationCode()
		{
			var random = new Random();
			return random.Next(100000, 999999).ToString();
		}

		public async Task<List<UserWithRolesDto>> GetAllUsersWithRolesAsync(CancellationToken cancellationToken)
		{
			var users = await _userManager.Users.ToListAsync(cancellationToken);
			var userWithRolesList = new List<UserWithRolesDto>();

			foreach (var user in users)
			{
				var roles = await _userManager.GetRolesAsync(user);
				userWithRolesList.Add(new UserWithRolesDto
				{
					UserId = user.Id.ToString(),
					Username = user.UserName,
					Email = user.Email,
					Roles = roles.ToList()
				});
			}

			return userWithRolesList;
		}

		public async Task<IdentityResult> UpdateProfileAsync(string userId, UpdateProfileDto updateProfileDto, CancellationToken cancellationToken)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return IdentityResult.Failed(new IdentityError { Description = "User not found." });
			}

			user.FirstName = updateProfileDto.FirstName;
			user.LastName = updateProfileDto.LastName;
			user.Address = updateProfileDto.Address;
			user.PhoneNumber = updateProfileDto.PhoneNumber;

			var result = await _userManager.UpdateAsync(user);

			return result;
		}

		public async Task<AuthenticationProperties> ExternalLoginAsync(string provider, string returnUrl = null)
		{
			var redirectUrl = returnUrl ?? "/external-login-callback";
			var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
			return properties;
		}

		public async Task<LoginResponseDto> ExternalLoginCallbackAsync()
		{
			var info = await _signInManager.GetExternalLoginInfoAsync();
			if (info == null)
			{
				return null;
			}

			var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
			if (signInResult.Succeeded)
			{
				var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
				if (user == null)
				{
					return null;
				}

				var roles = await _userManager.GetRolesAsync(user);
				var token = _jwtHelper.GenerateJwtToken(user, roles);
				var response = _mapper.Map<LoginResponseDto>(user);
				response.Token = token;
				response.Roles = roles.ToList();

				return response;
			}

			var email = info.Principal.FindFirstValue(ClaimTypes.Email);
			if (email != null)
			{
				var user = await _userManager.FindByEmailAsync(email);
				if (user == null)
				{
					user = new AppUser
					{
						UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
						Email = info.Principal.FindFirstValue(ClaimTypes.Email)
					};
					await _userManager.CreateAsync(user);
				}

				await _userManager.AddLoginAsync(user, info);
				await _signInManager.SignInAsync(user, isPersistent: false);

				var roles = await _userManager.GetRolesAsync(user);
				var token = _jwtHelper.GenerateJwtToken(user, roles);
				var response = _mapper.Map<LoginResponseDto>(user);
				response.Token = token;
				response.Roles = roles.ToList();

				return response;
			}

			return null;
		}
	}
}
