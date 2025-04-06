namespace QuestRoomMVC.WebMVC.ViewModel;
public class MyBookingViewModel
{
    public int BookingId { get; set; }
    public string RoomName { get; set; }
    public string? RoomImageUrl { get; set; } // нове
    public DateTime EndTime { get; set; }
    public int RoomId { get; set; }
    public bool AlreadyRated { get; set; }
    public int? ExistingRating { get; set; }
}
