using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuestRoomMVC.Domain.Entities;
using QuestRoomMVC.Infrastracture;
using QuestRoomMVC.WebMVC.ViewModel;
using SelectPdf;
using IronBarCode;
using static System.Net.WebRequestMethods;

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
        public async Task<IActionResult> MyBookings()
        {
            var applicationUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(applicationUserId))
                return Unauthorized();

            var user = await _context.User
                .Include(u => u.ApplicationUser)
                .FirstOrDefaultAsync(u => u.ApplicationUserId == applicationUserId);

            if (user == null)
                return NotFound($"User not found for ApplicationUserId = {applicationUserId}");

            var bookings = await _context.Booking
                .Include(b => b.Schedule)
                    .ThenInclude(s => s.Room)
                .Where(b => b.UserId == user.Id)
                .ToListAsync();

            var ratings = await _context.Rating
                .Where(r => r.UserId == user.Id)
                .ToListAsync();

            var viewModel = bookings.Select(b => new MyBookingViewModel
            {
                BookingId = b.Id,
                RoomName = b.Schedule.Room.Name,
                RoomImageUrl = b.Schedule.Room.Image,
                EndTime = b.Schedule.EndTime,
                AlreadyRated = ratings.Any(r => r.RoomId == b.Schedule.RoomId),
                ExistingRating = ratings.FirstOrDefault(r => r.RoomId == b.Schedule.RoomId)?.Score,
                RoomId = b.Schedule.RoomId
            }).ToList();

            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Rate(int roomId, int score)
        {
            var applicationUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(applicationUserId))
                return Unauthorized();

            var user = await _context.User
                .FirstOrDefaultAsync(u => u.ApplicationUserId == applicationUserId);

            if (user == null)
                return NotFound();

            var booking = await _context.Booking
                .Include(b => b.Schedule)
                .FirstOrDefaultAsync(b => b.UserId == user.Id && b.Schedule.RoomId == roomId);

            if (booking == null || booking.Schedule.EndTime > DateTime.UtcNow)
                return BadRequest("Неможливо оцінити до завершення бронювання");

            var alreadyRated = await _context.Rating.AnyAsync(r => r.UserId == user.Id && r.RoomId == roomId);
            if (alreadyRated)
                return BadRequest("Ви вже оцінили цю кімнату");

            var rating = new Rating
            {
                UserId = user.Id,
                RoomId = roomId,
                Score = score,
                CreatedAt = DateTime.UtcNow
            };

            _context.Rating.Add(rating);
            await _context.SaveChangesAsync();

            return RedirectToAction("MyBookings");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int bookingId)
        {
            var booking = await _context.Booking
                .Include(b => b.Schedule)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null)
                return NotFound();

            if (booking.Schedule.EndTime <= DateTime.UtcNow)
                return BadRequest("Неможливо скасувати вже завершене бронювання.");
            booking.Schedule.IsBooked = false;
            _context.Booking.Remove(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction("MyBookings");
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
        public IActionResult GetQrCode(int id)
        {
            var url = "https://i.pinimg.com/236x/9d/40/4f/9d404f1a2f8ec68879aacac3c3c95f50.jpg";
            //var url = $"{Request.Scheme}://{Request.Host}/Bookings/PdfView/{id}";
            var qrCode = QRCodeWriter.CreateQrCode(url, 150);
            qrCode.SetMargins(10);
            qrCode.ChangeBackgroundColor(System.Drawing.Color.White);
            qrCode.ChangeBarCodeColor(System.Drawing.Color.Black);
            var stream = qrCode.ToStream();
            return File(stream.ToArray(), "image/png");
        }
        [HttpGet]
        public IActionResult DownloadBookingPdf(int id)
        {
            var url = $"{Request.Scheme}://{Request.Host}/Bookings/PdfView/{id}";

            var converter = new SelectPdf.HtmlToPdf();
            converter.Options.PdfPageSize = PdfPageSize.A5;
            converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
            converter.Options.MarginTop = 20;
            converter.Options.MarginBottom = 20;
            converter.Options.MarginLeft = 30;
            converter.Options.MarginRight = 30;
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
            if (!User.Identity.IsAuthenticated)
            {
                // redirect до Login з поверненням сюди після логіну
                var returnUrl = Url.Action("CreateBooking", "Bookings", new { scheduleId });
                return RedirectToAction("Login", "Account", new { returnUrl });
            }

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
                    return View("Create", booking);
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
