using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestRoomMVC.Domain.Entities;

public class User : Entity
{
    public string ApplicationUserId { get; set; } = null!;
    public ApplicationUser ApplicationUser { get; set; }= null!;
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
}
