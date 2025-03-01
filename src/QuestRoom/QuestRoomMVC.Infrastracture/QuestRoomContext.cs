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

        // Seed Locations
        /*modelBuilder.Entity<Location>().HasData(
            new Location { Id = 1, Name = "ТРЦ \"Galaxy Mall\"" },
            new Location { Id = 2, Name = "Станція метро \"Центральна\"" }
        );

        // Seed Genres
        modelBuilder.Entity<Genre>().HasData(
            new Genre { Id = 3, Name = "Пригоди" },
            new Genre { Id = 4, Name = "Жахи" },
            new Genre { Id = 5, Name = "Фантастика" },
            new Genre { Id = 6, Name = "Детектив" },
            new Genre { Id = 7, Name = "Історія" }
        );

        modelBuilder.Entity<Room>().HasData(
        new Room
        {
            Id = 1,
            Name = "Центральний підрозділ",
            LocationId = 2,
            GenreId = 6, // Детектив
            MaxPlayers = 6,
            Difficulty = 3,
            Description = "Ваша команда агентів має розслідувати таємниче зникнення в центральному відділі.",
            Image = "central_department.jpg",
            CreatedAt = DateTime.UtcNow
        },
        new Room
        {
            Id = 2,
            Name = "Таємний бункер",
            LocationId = 1,
            GenreId = 4, // Жахи
            MaxPlayers = 5,
            Difficulty = 5,
            Description = "Покинутий бункер приховує страшні секрети. Чи встигнете вибратися?",
            Image = "secret_bunker.jpg",
            CreatedAt = DateTime.UtcNow
        },
        new Room
        {
            Id = 3,
            Name = "Зона експериментів",
            LocationId = 1,
            GenreId = 5, // Фантастика
            MaxPlayers = 4,
            Difficulty = 4,
            Description = "Лабораторія, де проводили небезпечні експерименти. Тепер ви їх частина.",
            Image = "experiment_zone.jpg",
            CreatedAt = DateTime.UtcNow
        }
        );*/
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
