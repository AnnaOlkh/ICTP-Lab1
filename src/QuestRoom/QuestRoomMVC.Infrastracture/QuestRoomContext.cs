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
    public DbSet<User> Users { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Schedule> Schedules { get; set; }
    public DbSet<Rating> Ratings { get; set; }
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
