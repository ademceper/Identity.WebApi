using Identity.WebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.WebApi.Configurations
{
	/// <summary>
	/// Provides configuration for the VerificationCode entity.
	/// </summary>
	public class PasswordResetCodeConfiguration : IEntityTypeConfiguration<PasswordResetCode>
	{
		public void Configure(EntityTypeBuilder<PasswordResetCode> builder)
		{
			builder.HasKey(e => e.Id);
			builder.Property(e => e.Email).IsRequired().HasMaxLength(256);
			builder.Property(e => e.Code).IsRequired().HasMaxLength(6);
			builder.Property(e => e.CreatedAt).IsRequired();
			builder.Property(e => e.ExpiryTime).IsRequired();
		}
	}
}
