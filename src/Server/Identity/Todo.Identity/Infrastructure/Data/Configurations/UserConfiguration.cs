using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo.Identity.Domain.Entities;

namespace Todo.Identity.Infrastructure.Data.Configurations
{
    internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.OwnsOne(u => u.FullName, fn =>
            {
                fn.Property(p => p.FirstName)
                    .HasColumnName("FirstName")
                    .IsRequired()
                    .HasMaxLength(100);

                fn.Property(p => p.LastName)
                    .HasColumnName("LastName")
                    .IsRequired()
                    .HasMaxLength(100);
            });

            builder.OwnsOne(u => u.EmailAddress, e =>
            {
                e.Property(p => p.Value)
                    .HasColumnName("Email")
                    .IsRequired()
                    .HasMaxLength(256);

                e.HasIndex(p => p.Value)
                    .IsUnique()
                    .HasDatabaseName("IX_Users_Email");
            });

            builder.OwnsOne(u => u.PhoneNumber, pn =>
            {
                pn.Property(p => p.Value)
                    .HasColumnName("PhoneNumber")
                    .HasMaxLength(20);
            });

            builder.Property(u => u.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.Property(u => u.UpdatedAt)
                .HasColumnType("datetime2");

            builder.Property(u => u.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(u => u.RefreshToken)
                .HasMaxLength(512);

            builder.Property(u => u.RefreshTokenExpiryTime)
                .HasColumnType("datetime2");

            builder.HasIndex(u => u.NormalizedUserName)
                .IsUnique()
                .HasDatabaseName("IX_Users_NormalizedUserName");

            builder.HasIndex(u => u.RefreshToken)
                .HasDatabaseName("IX_Users_RefreshToken");

            builder.HasMany<IdentityUserRole<Guid>>()
                .WithOne()
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();
        }
    }
}