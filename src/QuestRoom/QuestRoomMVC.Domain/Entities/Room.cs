using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestRoomMVC.Domain.Entities
{
    public class Room : Entity, IAggregateRoot
    {
        [MaxLength(50)]
        public required string Name { get; set; }
        [Display (Name = "Location")]
        public int LocationId { get; set; }

        public Location? Location { get; set; }
        [Display (Name = "Genre")]
        public int GenreId { get; set; }

        public Genre? Genre { get; set; }
        [Display (Name = "Number of players")]
        [Range(1, 20)]
        public int MaxPlayers { get; set; }

        [Range(1, 5)]
        public required int Difficulty { get; set; }

        [MaxLength(5000)]
        public string? Description { get; set; }
        public string? Image { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

    }
}
