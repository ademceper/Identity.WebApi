using Identity.WebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.WebApi.Configurations
{
	/// <summary>
	/// Configures the properties and relationships of the AppUser entity.
	/// </summary>
	public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
	{
		/// <summary>
		/// Configures the AppUser entity.
		/// </summary>
		/// <param name="builder">The builder being used to configure the AppUser entity.</param>
		public void Configure(EntityTypeBuilder<AppUser> builder)
		{
			// Configure UserName to be unique
			builder.HasIndex(u => u.UserName).IsUnique();
			// Configure Email to be unique
			builder.HasIndex(u => u.Email).IsUnique();
		}
	}
}
