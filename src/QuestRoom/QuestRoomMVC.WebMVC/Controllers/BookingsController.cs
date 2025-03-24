using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuestRoomMVC.Domain.Entities;
using QuestRoomMVC.Infrastracture;

namespace QuestRoomMVC.WebMVC.Controllers
{
    public class BookingsController : Controller
    {
        private readonly QuestRoomContext _context;

        public BookingsController(QuestRoomContext context)
        {
            _context = context;
        }
        public IActionResult Confirmed()
        {
            return View();
        }
        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            var questRoomContext = _context.Booking.Include(b => b.Schedule).Include(b => b.User);
            return View(await questRoomContext.ToListAsync());
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.Schedule)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }
        [HttpGet]
        public async Task<IActionResult> CreateBooking(int scheduleId)
        {
            var schedule = await _context.Schedule
                .Include(s => s.Room)
                .FirstOrDefaultAsync(s => s.Id == scheduleId);

            if (schedule == null)
            {
                return NotFound("Розклад не знайдено.");
            }

            if (schedule.IsBooked || schedule.Booking != null)
            {
                TempData["ErrorMessage"] = "Цей час уже заброньовано.";
                return RedirectToAction("Index", "Schedule");
            }

            ViewBag.Schedule = schedule;
            return View("Create", new Booking { ScheduleId = scheduleId, PlayersNumber = 1 });
        }
        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> PostBooking([Bind("ScheduleId,PlayersNumber,Comment")] Booking booking)
        {
            var schedule = await _context.Schedule
                .Include(s => s.Booking)
                .FirstOrDefaultAsync(s => s.Id == booking.ScheduleId);

            if (schedule == null)
            {
                ModelState.AddModelError("", "Розклад не знайдено.");
                return View("Create", booking);
            }

            if (schedule.IsBooked || schedule.Booking != null)
            {
                ModelState.AddModelError("", "Цей час уже заброньовано.");
                ViewBag.Schedule = schedule;
                return View("Create", booking);
            }

            if (ModelState.IsValid)
            {
                var appUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var user = await _context.User
                    .FirstOrDefaultAsync(u => u.ApplicationUserId == appUserId);

                if (user == null)
                {
                    ModelState.AddModelError("", "Користувача не знайдено.");
                    ViewBag.Schedule = schedule;
                    return View(booking);
                }
                var room = await _context.Room
                .FirstOrDefaultAsync(r => r.Id == schedule.RoomId);

                if (room != null && booking.PlayersNumber > room.MaxPlayers)
                {
                    ModelState.AddModelError(nameof(booking.PlayersNumber), $"Максимальна кількість гравців: {room.MaxPlayers}");
                    ViewBag.Schedule = schedule;
                    return View("Create", booking);
                }
                booking.UserId = user.Id;


                booking.UserId = user.Id;
                booking.CreatedAt = DateTime.UtcNow;

                schedule.IsBooked = true;
                schedule.Booking = booking;

                _context.Booking.Add(booking);
                _context.Schedule.Update(schedule);

                await _context.SaveChangesAsync();

                return RedirectToAction("Confirmed");
            }

            ViewBag.Schedule = schedule;
            return View("Create", booking);
        }


        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["ScheduleId"] = new SelectList(_context.Schedule, "Id", "Id", booking.ScheduleId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "ApplicationUserId", booking.UserId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,ScheduleId,PlayersNumber,Comment,CreatedAt,Id")] Booking booking)
        {
            if (id != booking.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ScheduleId"] = new SelectList(_context.Schedule, "Id", "Id", booking.ScheduleId);
            ViewData["UserId"] = new SelectList(_context.User, "Id", "ApplicationUserId", booking.UserId);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking
                .Include(b => b.Schedule)
                .Include(b => b.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Booking.FindAsync(id);
            if (booking != null)
            {
                _context.Booking.Remove(booking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Booking.Any(e => e.Id == id);
        }
    }
}
