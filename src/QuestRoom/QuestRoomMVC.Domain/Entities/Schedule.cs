using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestRoomMVC.Domain.Entities
{
    public class Schedule : Entity
    {
        public int ID { get; set; }
        public int RoomId { get; set; }
        public Room? Room { get; set; }
        public required DateTime StartTime { get; set; }
        public required DateTime EndTime { get; set; }
        public bool IsBooked { get; set; } = false;

        [Range(0, int.MaxValue)]
        public int Price { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
