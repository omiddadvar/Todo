using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo.Identity.Domain.Entities;

namespace Todo.Identity.Infrastructure.Data.Configurations;

internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.Property(r => r.Description)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(r => r.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(r => r.UpdatedAt)
            .HasColumnType("datetime2");

        builder.HasIndex(r => r.NormalizedName)
            .IsUnique()
            .HasDatabaseName("IX_Roles_NormalizedName");

        builder.HasMany<IdentityUserRole<Guid>>()
            .WithOne()
            .HasForeignKey(ur => ur.RoleId)
            .IsRequired();
    }
}