using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestRoomMVC.Domain.Entities
{
    public class Genre : Entity
    {
        public int ID { get; set; }
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
    }
}
