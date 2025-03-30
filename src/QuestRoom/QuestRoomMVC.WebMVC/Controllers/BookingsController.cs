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
using SelectPdf;

namespace QuestRoomMVC.WebMVC.Controllers
{
    public class BookingsController : Controller
    {
        private readonly QuestRoomContext _context;

        public BookingsController(QuestRoomContext context)
        {
            _context = context;
        }
        public IActionResult Confirmed(int id)
        {
            return View(id);
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
        public IActionResult DownloadBookingPdf(int id)
        {
            var url = $"{Request.Scheme}://{Request.Host}/Bookings/PdfView/{id}";

            var converter = new SelectPdf.HtmlToPdf();
            // Задати фіксований розмір сторінки (наприклад, A5 або власний)
            converter.Options.PdfPageSize = PdfPageSize.A5;
            converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;

            // Встановити поля сторінки (у пікселях)
            converter.Options.MarginTop = 20;
            converter.Options.MarginBottom = 20;
            converter.Options.MarginLeft = 30;
            converter.Options.MarginRight = 30;

            // (необов’язково) Фіксована ширина веб-сторінки для рендеру
            converter.Options.WebPageWidth = 800;

            converter.Options.PdfPageSize = SelectPdf.PdfPageSize.A4;
            converter.Options.PdfPageOrientation = SelectPdf.PdfPageOrientation.Portrait;

            var doc = converter.ConvertUrl(url);
            var pdf = doc.Save();
            doc.Close();

            return File(pdf, "application/pdf", $"booking_{id}.pdf");
        }

        [HttpGet]
        public async Task<IActionResult> PdfView(int id)
        {
            var booking = await _context.Booking
                .Include(b => b.User).ThenInclude(u => u.ApplicationUser)
                .Include(b => b.Schedule).ThenInclude(s => s.Room)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null) return NotFound();

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

                return RedirectToAction("Confirmed", new { id = booking.Id });
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
