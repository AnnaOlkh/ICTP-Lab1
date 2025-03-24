using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestRoomMVC.Infrastracture;

namespace QuestRoomMVC.WebMVC.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChartsAPIController : ControllerBase
{
    private readonly QuestRoomContext _context;

    public ChartsAPIController(QuestRoomContext context)
    {
        _context = context;
    }

    public record BookingsByTimeItem(int Day, int Hour, int Count);
    public record BookingsByRoomItem(string RoomName, int Count);

    [HttpGet("bookings-by-day-and-hour")]
    public async Task<IActionResult> GetBookingsByDayAndHourAsync(CancellationToken cancellationToken)
    {
        var data = await _context.Schedule
            .Where(s => s.IsBooked)
            .ToListAsync(cancellationToken);

        var result = data
            .GroupBy(s => new { Day = (int)s.StartTime.DayOfWeek, Hour = s.StartTime.Hour })
            .Select(g => new BookingsByTimeItem(g.Key.Day, g.Key.Hour, g.Count()))
            .ToList();

        return Ok(result);
    }
    [HttpGet("bookings-by-day")]
    public async Task<IActionResult> GetBookingsByDayAsync(CancellationToken cancellationToken)
    {
        var data = await _context.Schedule
            .Where(s => s.IsBooked)
            .ToListAsync(cancellationToken);

        var result = data
            .GroupBy(s => (int)s.StartTime.DayOfWeek)
            .Select(g => new
            {
                Day = g.Key, // 0 = Нд, 1 = Пн, ...
                Count = g.Count()
            })
            .OrderBy(r => r.Day)
            .ToList();

        return Ok(result);
    }


    [HttpGet("bookings-by-room")]
    public async Task<IActionResult> GetBookingsByRoomAsync(CancellationToken cancellationToken)
    {
        var data = await _context.Schedule
            .Include(s => s.Room)
            .Where(s => s.IsBooked && s.Room != null)
            .ToListAsync(cancellationToken);

        var result = data
            .GroupBy(s => s.Room!.Name)
            .Select(g => new BookingsByRoomItem(g.Key, g.Count()))
            .ToList();

        return Ok(result);
    }
}
