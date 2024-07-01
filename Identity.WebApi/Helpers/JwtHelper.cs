using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Identity.WebApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Identity.WebApi.Helpers
{
	/// <summary>
	/// Defines the contract for JWT token generation.
	/// </summary>
	public interface IJwtHelper
	{
		/// <summary>
		/// Generates a JWT token for the specified user.
		/// </summary>
		/// <param name="user">The user for whom the token is generated.</param>
		/// <returns>A JWT token as a string.</returns>
		string GenerateJwtToken(AppUser user);
	}

	/// <summary>
	/// Provides methods for generating JWT tokens.
	/// </summary>
	public class JwtHelper : IJwtHelper
	{
		private readonly IConfiguration _configuration;

		/// <summary>
		/// Initializes a new instance of the <see cref="JwtHelper"/> class.
		/// </summary>
		/// <param name="configuration">The application configuration settings.</param>
		public JwtHelper(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		/// <summary>
		/// Generates a JWT token for the specified user.
		/// </summary>
		/// <param name="user">The user for whom the token is generated.</param>
		/// <returns>A JWT token as a string.</returns>
		public string GenerateJwtToken(AppUser user)
		{
			var jwtSettings = _configuration.GetSection("JwtSettings");
			var secretKey = jwtSettings["Secret"];
			var issuer = jwtSettings["Issuer"];
			var audience = jwtSettings["Audience"];
			var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"]);

			var claims = new List<Claim>
			{
				new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new Claim(ClaimTypes.Name, user.UserName)
			};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer,
				audience,
				claims,
				expires: DateTime.Now.AddMinutes(expiryMinutes),
				signingCredentials: creds);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
