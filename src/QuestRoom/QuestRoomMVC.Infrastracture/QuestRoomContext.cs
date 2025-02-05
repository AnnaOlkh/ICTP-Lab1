using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestRoomMVC.Domain.Entities;
using QuestRoomMVC.Infrastracture.EntityConfigurations;

namespace QuestRoomMVC.Infrastracture;

public class QuestRoomContext : DbContext
{
    public QuestRoomContext(DbContextOptions<QuestRoomContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new RoomEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new GenreEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new LocationEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new BookingEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ScheduleEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new RatingEntityTypeConfiguration());
    }

}
