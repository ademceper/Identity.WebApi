using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Identity.WebApi.Validators
{
	public class CustomPasswordValidator<TUser> : IPasswordValidator<TUser> where TUser : class
	{
		public Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, string password)
		{
			var errors = new List<IdentityError>();

			if (string.IsNullOrEmpty(password) || password.Length < 8)
			{
				errors.Add(new IdentityError
				{
					Code = "PasswordTooShort",
					Description = "Password must be at least 8 characters long."
				});
			}

			if (!HasUppercaseLetter(password))
			{
				errors.Add(new IdentityError
				{
					Code = "PasswordRequiresUpper",
					Description = "Password must contain at least one uppercase letter."
				});
			}

			if (!HasLowercaseLetter(password))
			{
				errors.Add(new IdentityError
				{
					Code = "PasswordRequiresLower",
					Description = "Password must contain at least one lowercase letter."
				});
			}

			if (!HasDigit(password))
			{
				errors.Add(new IdentityError
				{
					Code = "PasswordRequiresDigit",
					Description = "Password must contain at least one digit."
				});
			}

			if (!HasSpecialCharacter(password))
			{
				errors.Add(new IdentityError
				{
					Code = "PasswordRequiresNonAlphanumeric",
					Description = "Password must contain at least one special character."
				});
			}

			return Task.FromResult(errors.Count == 0 ? IdentityResult.Success : IdentityResult.Failed(errors.ToArray()));
		}

		private bool HasUppercaseLetter(string password) => password.IndexOfAny("ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray()) != -1;

		private bool HasLowercaseLetter(string password) => password.IndexOfAny("abcdefghijklmnopqrstuvwxyz".ToCharArray()) != -1;

		private bool HasDigit(string password) => password.IndexOfAny("0123456789".ToCharArray()) != -1;

		private bool HasSpecialCharacter(string password) => password.IndexOfAny("!@#$%^&*()_+-=[]{}|;':\",.<>?/`~".ToCharArray()) != -1;
	}
}
