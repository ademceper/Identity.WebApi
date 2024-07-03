using FluentValidation;

public class RegisterValidator : AbstractValidator<RegisterDto>
{
	public RegisterValidator()
	{
		RuleFor(x => x.Email)
			.NotEmpty().WithMessage("Email is required.")
			.EmailAddress().WithMessage("Invalid email format.")
			.Must(NotContainSqlInjection).WithMessage("Invalid email format.")
			.Must(NotContainJavaScript).WithMessage("Invalid email format.");

		RuleFor(x => x.Password)
			.NotEmpty().WithMessage("Password is required.")
			.MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
			.Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
			.Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
			.Matches(@"[0-9]").WithMessage("Password must contain at least one number.")
			.Matches(@"[\!\?\*\.]").WithMessage("Password must contain at least one special character.")
			.Must(NotContainSqlInjection).WithMessage("Invalid password format.")
			.Must(NotContainJavaScript).WithMessage("Invalid password format.");

		RuleFor(x => x.ConfirmPassword)
			.Equal(x => x.Password).WithMessage("Passwords do not match.")
			.Must(NotContainSqlInjection).WithMessage("Invalid confirm password format.")
			.Must(NotContainJavaScript).WithMessage("Invalid confirm password format.");

		RuleFor(x => x.Username)
			.NotEmpty().WithMessage("Username is required.")
			.Must(NotContainSqlInjection).WithMessage("Invalid username format.")
			.Must(NotContainJavaScript).WithMessage("Invalid username format.");

		RuleFor(x => x.FirstName)
			.NotEmpty().WithMessage("First name is required.")
			.Must(NotContainSqlInjection).WithMessage("Invalid first name format.")
			.Must(NotContainJavaScript).WithMessage("Invalid first name format.");

		RuleFor(x => x.LastName)
			.NotEmpty().WithMessage("Last name is required.")
			.Must(NotContainSqlInjection).WithMessage("Invalid last name format.")
			.Must(NotContainJavaScript).WithMessage("Invalid last name format.");
	}

	private bool NotContainSqlInjection(string input)
	{
		string[] sqlInjectionPatterns = { "select ", "insert ", "delete ", "update ", "drop ", "--", ";" };
		return !sqlInjectionPatterns.Any(pattern => input.ToLower().Contains(pattern));
	}

	private bool NotContainJavaScript(string input)
	{
		string[] jsInjectionPatterns = { "<script>", "</script>", "javascript:", "alert(", "onload=" };
		return !jsInjectionPatterns.Any(pattern => input.ToLower().Contains(pattern));
	}
}
