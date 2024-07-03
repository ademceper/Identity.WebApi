using System.Collections.Generic;
using Identity.WebApi.Models;

namespace Identity.WebApi.Helpers
{
	public interface IJwtHelper
	{
		string GenerateJwtToken(AppUser user, IList<string> roles);
	}
}
