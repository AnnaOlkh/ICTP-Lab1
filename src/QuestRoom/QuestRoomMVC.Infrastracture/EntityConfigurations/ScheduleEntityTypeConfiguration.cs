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
    internal class ScheduleEntityTypeConfiguration : IEntityTypeConfiguration<Schedule>
    {
        public void Configure(EntityTypeBuilder<Schedule> builder)
        {
            builder.HasKey(schedule => schedule.ID);
            builder.Property(schedule => schedule.ID).UseIdentityColumn();

            builder.Property(schedule => schedule.StartTime)
                .IsRequired();

            builder.Property(schedule => schedule.EndTime)
                .IsRequired();

            builder.Property(schedule => schedule.IsBooked)
                .IsRequired();

            builder.Property(schedule => schedule.Price)
                .IsRequired();
            builder.Property(schedule => schedule.CreatedAt)
                .IsRequired();

            builder.HasOne(schedule => schedule.Room)
                .WithMany(room => room.Schedules)
                .HasForeignKey(schedule => schedule.RoomId);
        }
    }
}
