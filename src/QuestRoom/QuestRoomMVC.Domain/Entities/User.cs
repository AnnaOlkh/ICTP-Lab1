using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestRoomMVC.Domain.Entities
{
    public class User : Entity, IAggregateRoot
    {
        public enum UserRole
        {
            Admin,
            Сustomer
        }
        public int ID { get; set; }

        [MaxLength(50)]
        public required string FirstName { get; set; }

        [MaxLength(50)]
        public required string LastName { get; set; }
        public UserRole Role { get; set; } = UserRole.Сustomer;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    }
}
