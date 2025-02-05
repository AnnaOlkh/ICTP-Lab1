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
            builder.HasKey(booking => booking.Id);

            builder.Property(booking => booking.PlayersNumber)
                .IsRequired();

            builder.Property(booking => booking.Comment)
                .HasMaxLength(1000);

            builder.Property(booking => booking.CreatedAt)
                .IsRequired();

            builder.HasOne(booking => booking.User)
                .WithMany(user => user.Bookings)
                .HasForeignKey(booking => booking.UserId);

            builder.HasOne(booking => booking.Schedule)
                 .WithOne(schedule => schedule.Booking)
                 .HasForeignKey<Booking>(booking => booking.ScheduleId);
        }
    }
}
