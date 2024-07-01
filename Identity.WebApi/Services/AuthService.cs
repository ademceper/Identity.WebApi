using Identity.WebApi.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using Identity.WebApi.Helpers;
using Identity.WebApi.Context;
using Microsoft.EntityFrameworkCore;

namespace Identity.WebApi.Services
{
	public class AuthService : IAuthService
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;
		private readonly IMapper _mapper;
		private readonly IJwtHelper _jwtHelper;
		private readonly IEmailService _emailService;
		private readonly AppDbContext _context;

		/// <summary>
		/// Initializes a new instance of the <see cref="AuthService"/> class.
		/// </summary>
		/// <param name="userManager">User manager service.</param>
		/// <param name="signInManager">Sign-in manager service.</param>
		/// <param name="mapper">Mapper service.</param>
		/// <param name="jwtHelper">JWT helper service.</param>
		/// <param name="emailService">Email service.</param>
		/// <param name="context">Database context.</param>
		public AuthService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IMapper mapper, IJwtHelper jwtHelper, IEmailService emailService, AppDbContext context)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_mapper = mapper;
			_jwtHelper = jwtHelper;
			_emailService = emailService;
			_context = context;
		}

		/// <summary>
		/// Registers a new user.
		/// </summary>
		/// <param name="registerDto">The registration DTO containing user details.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>IdentityResult representing the result of the registration process.</returns>
		public async Task<IdentityResult> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken)
		{
			if (registerDto.Password != registerDto.ConfirmPassword)
			{
				return IdentityResult.Failed(new IdentityError { Description = "Passwords do not match." });
			}

			var existingUserByUsername = await _userManager.FindByNameAsync(registerDto.Username);
			if (existingUserByUsername != null)
			{
				return IdentityResult.Failed(new IdentityError { Description = "Username is already taken." });
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

		/// <summary>
		/// Logs in a user.
		/// </summary>
		/// <param name="loginDto">The login DTO containing user credentials.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>LoginResponseDto containing the user details and JWT token.</returns>
		public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken)
		{
			var user = await _userManager.FindByNameAsync(loginDto.Username);
			if (user == null)
			{
				return null;
			}

			var result = await _signInManager.PasswordSignInAsync(user.UserName, loginDto.Password, isPersistent: false, lockoutOnFailure: false);
			if (!result.Succeeded)
			{
				return null;
			}

			var token = _jwtHelper.GenerateJwtToken(user);

			var response = _mapper.Map<LoginResponseDto>(user);
			response.Token = token;

			return response;
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

		/// <summary>
		/// Changes the password of a user.
		/// </summary>
		/// <param name="userId">The ID of the user whose password is to be changed.</param>
		/// <param name="changePasswordDto">The DTO containing the current and new passwords.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>IdentityResult representing the result of the password change.</returns>
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

			return result;
		}

		/// <summary>
		/// Initiates the password reset process by sending a reset code to the user's email.
		/// </summary>
		/// <param name="forgotPasswordRequestDto">The DTO containing the user's email.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>IdentityResult representing the result of the password reset request.</returns>
		public async Task<IdentityResult> ForgotPasswordAsync(ForgotPasswordRequestDto forgotPasswordRequestDto, CancellationToken cancellationToken)
		{
			var user = await _userManager.FindByEmailAsync(forgotPasswordRequestDto.Email);
			if (user == null)
			{
				return IdentityResult.Failed(new IdentityError { Description = "User not found." });
			}

			var code = GenerateVerificationCode();
			var passwordResetCode = new PasswordResetCode
			{
				Email = forgotPasswordRequestDto.Email,
				Code = code,
				CreatedAt = DateTime.UtcNow
			};

			_context.PasswordResetCodes.Add(passwordResetCode);
			await _context.SaveChangesAsync(cancellationToken);

			await _emailService.SendEmailAsync(forgotPasswordRequestDto.Email, "Password Reset Code", $"Your password reset code is: {code}");

			return IdentityResult.Success;
		}

		/// <summary>
		/// Resets the user's password using the provided reset code.
		/// </summary>
		/// <param name="resetPasswordDto">The DTO containing the email, reset code, and new password.</param>
		/// <param name="cancellationToken">Cancellation token.</param>
		/// <returns>IdentityResult representing the result of the password reset.</returns>
		public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto, CancellationToken cancellationToken)
		{
			var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
			if (user == null)
			{
				return IdentityResult.Failed(new IdentityError { Description = "User not found." });
			}

			var storedCode = await _context.PasswordResetCodes.FirstOrDefaultAsync(c => c.Email == resetPasswordDto.Email && c.Code == resetPasswordDto.Code);
			if (storedCode == null)
			{
				return IdentityResult.Failed(new IdentityError { Description = "Invalid code." });
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

		/// <summary>
		/// Generates a random verification code.
		/// </summary>
		/// <returns>A six-digit verification code as a string.</returns>
		private string GenerateVerificationCode()
		{
			var random = new Random();
			return random.Next(100000, 999999).ToString();
		}
	}
}
