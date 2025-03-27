using MakeWish.UserService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MakeWish.UserService.Adapters.DataAccess.EntityFramework.Configurations;

internal sealed class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder
            .Property(user => user.Id)
            .HasColumnName("id")
            .HasColumnType("varchar")
            .HasMaxLength(50)
            .IsRequired();

        builder
            .Property(user => user.Email)
            .HasColumnName("email")
            .HasColumnType("varchar")
            .HasMaxLength(64)
            .IsRequired();
        
        builder
            .Property(user => user.PasswordHash)
            .HasColumnName("password_hash")
            .HasColumnType("varchar")
            .HasMaxLength(64)
            .IsRequired();

        builder
            .Property(user => user.Name)
            .HasColumnName("name")
            .HasColumnType("varchar")
            .HasMaxLength(64)
            .IsRequired();

        builder
            .Property(user => user.Surname)
            .HasColumnName("surname")
            .HasColumnType("varchar")
            .HasMaxLength(64)
            .IsRequired();

        builder.HasKey(user => user.Id);
        
        builder.HasIndex(user => user.Email).IsUnique();
    }
}