using Microsoft.AspNetCore.Identity;

public class CustomIdentityErrorDescriber : IdentityErrorDescriber
{
	public override IdentityError DuplicateUserName(string userName)
	{
		return new IdentityError
		{
			Code = nameof(DuplicateUserName),
			Description = $"Kullanıcı adı '{userName}' zaten alınmış."
		};
	}

	public override IdentityError DuplicateEmail(string email)
	{
		return new IdentityError
		{
			Code = nameof(DuplicateEmail),
			Description = $"Email '{email}' zaten alınmış."
		};
	}

	public override IdentityError PasswordTooShort(int length)
	{
		return new IdentityError
		{
			Code = nameof(PasswordTooShort),
			Description = $"Şifre en az {length} karakter olmalıdır."
		};
	}

}
