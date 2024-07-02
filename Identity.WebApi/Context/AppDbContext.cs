using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Identity.WebApi.Models;
using Identity.WebApi.Configurations;
using Microsoft.AspNetCore.Identity;

namespace Identity.WebApi.Context
{
	/// <summary>
	/// Represents the application's database context.
	/// </summary>
	public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AppDbContext"/> class.
		/// </summary>
		/// <param name="options">The options to be used by a <see cref="DbContext"/>.</param>
		public AppDbContext(DbContextOptions<AppDbContext> options)
			: base(options)
		{
		}

		/// <summary>
		/// Gets or sets the PasswordResetCodes DbSet.
		/// </summary>
		public DbSet<PasswordResetCode> PasswordResetCodes { get; set; }


		/// <summary>
		/// Gets or sets the VerificationCodes DbSet.
		/// </summary>
		public DbSet<VerificationCode> VerificationCodes { get; set; }

		/// <summary>
		/// Configures the model that was discovered by convention from the entity types
		/// exposed in <see cref="DbSet{TEntity}"/> properties on your derived context.
		/// </summary>
		/// <param name="builder">The builder being used to construct the model for this context.</param>
		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			// Apply configuration for AppUser entity
			builder.ApplyConfiguration(new AppUserConfiguration());

			// Apply configuration for VerificationCode entity
			builder.ApplyConfiguration(new VerificationCodeConfiguration());
		}
	}
}
