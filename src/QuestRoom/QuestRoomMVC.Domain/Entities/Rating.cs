﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestRoomMVC.Domain.Entities
{
    public class Rating : Entity
    {
        public int UserId { get; set; }
        public User? User { get; set; }
        public int RoomId { get; set; }
        public Room? Room { get; set; }
        [Range(1, 5)]
        public int Score { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
