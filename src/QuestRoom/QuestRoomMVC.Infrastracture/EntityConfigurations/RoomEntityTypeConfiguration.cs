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
    internal class RoomEntityTypeConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.HasKey(room => room.Id);
            builder.Property(room => room.Id).UseIdentityColumn();

            builder.Property(room => room.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(room => room.MaxPlayers)
                .IsRequired();

            builder.Property(room => room.Difficulty)
                .IsRequired();

            builder.Property(room => room.Image);

            builder.Property(room => room.Description)
                .HasMaxLength(5000);

            builder.Property(room => room.CreatedAt)
                .IsRequired();

            builder.Property(room => room.UpdatedAt);

            builder.HasOne(room => room.Location)
                .WithMany(location => location.Rooms)
                .HasForeignKey(room => room.LocationId);

            builder.HasOne(room => room.Genre)
                .WithMany(genre => genre.Rooms)
                .HasForeignKey(room => room.GenreId);
            builder.HasMany(room => room.Ratings)
                .WithOne(rating => rating.Room)
                .HasForeignKey(rating => rating.RoomId);

            builder.HasMany(room => room.Schedules)
                .WithOne(schedule => schedule.Room)
                .HasForeignKey(schedule => schedule.RoomId);
        }
    }
}
