using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestRoomMVC.Domain.Entities
{
    public class Location : Entity
    {
        [MaxLength(50)]
        public required string Name { get; set; }
        public ICollection<Room> Rooms { get; set; } = new List<Room>();

    }
}
