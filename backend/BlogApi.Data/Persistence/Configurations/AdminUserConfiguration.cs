using BlogApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogApi.Data.Persistence.Configurations;

public sealed class AdminUserConfiguration : IEntityTypeConfiguration<AdminUser>
{
  public void Configure(EntityTypeBuilder<AdminUser> entity)
  {
    entity.ToTable("AdminUsers");
    entity.HasKey(user => user.Id);
    entity.Property(user => user.Id).HasColumnName("id");
    entity.Property(user => user.Username).HasColumnName("username").IsRequired();
    entity.Property(user => user.Email).HasColumnName("email").IsRequired();
    entity.Property(user => user.PasswordHash).HasColumnName("password_hash").IsRequired();
    entity.HasIndex(user => user.Username).IsUnique();
    entity.HasIndex(user => user.Email).IsUnique();
  }
}
