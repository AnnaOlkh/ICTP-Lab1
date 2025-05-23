﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
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
            builder.HasKey(schedule => schedule.Id);
            builder.Property(schedule => schedule.Id).UseIdentityColumn();

            builder.Property(schedule => schedule.StartTime)
                .IsRequired();

            builder.Property(schedule => schedule.EndTime)
                .IsRequired();

            builder.Property(schedule => schedule.IsBooked)
                .IsRequired();

            builder.Property(schedule => schedule.Price)
                .IsRequired();
            builder.Property(schedule => schedule.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.HasOne(schedule => schedule.Room)
                .WithMany(room => room.Schedules)
                .HasForeignKey(schedule => schedule.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(schedule => schedule.Booking)
                .WithOne(booking => booking.Schedule)
                .HasForeignKey<Booking>(booking => booking.ScheduleId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
