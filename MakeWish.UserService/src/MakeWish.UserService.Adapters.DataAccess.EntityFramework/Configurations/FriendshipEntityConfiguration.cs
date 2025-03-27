using MakeWish.UserService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MakeWish.UserService.Adapters.DataAccess.EntityFramework.Configurations;

internal sealed class FriendshipEntityConfiguration : IEntityTypeConfiguration<Friendship>
{
    public void Configure(EntityTypeBuilder<Friendship> builder)
    {
        const string firstUserForeignKey = "first_user";
        const string secondUserForeignKey = "second_user";
        
        builder.ToTable("Friendships");
        
        builder
            .HasOne<User>(friendship => friendship.FirstUser)
            .WithMany()
            .HasForeignKey(firstUserForeignKey)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder
            .HasOne<User>(friendship => friendship.SecondUser)
            .WithMany()
            .HasForeignKey(secondUserForeignKey)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
        
        builder
            .Property(friendship => friendship.IsConfirmed)
            .HasColumnName("is_confirmed")
            .HasColumnType("bool")
            .HasDefaultValue(false)
            .IsRequired();
        
        builder.HasKey(firstUserForeignKey, secondUserForeignKey);

        builder.HasIndex(firstUserForeignKey);
        
        builder.HasIndex(secondUserForeignKey);
    }
}