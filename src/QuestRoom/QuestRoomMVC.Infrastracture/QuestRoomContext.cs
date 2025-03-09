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
    public DbSet<User> User { get; set; }
    public DbSet<Room> Room { get; set; }
    public DbSet<Genre> Genre { get; set; }
    public DbSet<Location> Location { get; set; }
    public DbSet<Booking> Booking { get; set; }
    public DbSet<Schedule> Schedule { get; set; }
    public DbSet<Rating> Rating { get; set; }
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
    public override int SaveChanges()
    {
        foreach (var entry in ChangeTracker.Entries<Room>())
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.Now;
            }
        }
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<Room>())
        {
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.Now;
            }
        }
        return await base.SaveChangesAsync(cancellationToken);
    }

}
