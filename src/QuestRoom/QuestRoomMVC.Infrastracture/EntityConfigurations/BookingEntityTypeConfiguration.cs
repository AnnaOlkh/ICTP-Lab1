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
    internal class BookingEntityTypeConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.HasKey(booking => booking.ID);

            builder.Property(booking => booking.PlayersNumber)
                .IsRequired();

            builder.Property(booking => booking.Comment)
                .HasMaxLength(1000);

            builder.Property(booking => booking.CreatedAt)
                .IsRequired();

            builder.HasOne(booking => booking.User)
                .WithMany(user => user.Bookings)
                .HasForeignKey(booking => booking.UserId);

            builder.HasOne(booking => booking.Room)
                .WithMany(room => room.Bookings)
                .HasForeignKey(booking => booking.RoomId);

            /*builder.HasOne(booking => booking.Schedule)
                .WithOne()

                .HasForeignKey<Booking>(booking => booking.ScheduleId);*/
           /* builder.HasOne(booking => booking.Schedule)
                .WithMany()
                .HasForeignKey(booking => booking.ScheduleId);*/
        }
    }
}
