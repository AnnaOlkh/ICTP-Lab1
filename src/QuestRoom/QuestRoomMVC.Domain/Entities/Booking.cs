using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestRoomMVC.Domain.Entities
{
    public class Booking : Entity
    {
        public int UserId { get; set; }
        public User? User { get; set; }
        public int ScheduleId { get; set; }
        public Schedule? Schedule { get; set; }

        [Range(1, 20)]
        public required int PlayersNumber { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
