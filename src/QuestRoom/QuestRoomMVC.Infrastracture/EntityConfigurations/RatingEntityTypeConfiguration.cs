using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using QuestRoomMVC.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestRoomMVC.Infrastracture.EntityConfigurations
{
    internal class RatingEntityTypeConfiguration : IEntityTypeConfiguration<Rating>
    {
        public void Configure(EntityTypeBuilder<Rating> builder)
        {
            builder.HasKey(rating => rating.Id);
            builder.Property(rating => rating.Id).UseIdentityColumn();

            builder.Property(rating => rating.Score)
                .IsRequired();

            builder.Property(rating => rating.CreatedAt)
                .IsRequired();

            builder.HasOne(rating => rating.User)
                .WithMany(user => user.Ratings)
                .HasForeignKey(rating => rating.UserId);

            builder.HasOne(rating => rating.Room)
                .WithMany(room => room.Ratings)
                .HasForeignKey(rating => rating.RoomId);
        }
    }
}
