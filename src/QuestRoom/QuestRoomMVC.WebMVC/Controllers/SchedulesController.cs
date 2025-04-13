using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuestRoomMVC.Domain.Entities;
using QuestRoomMVC.Infrastracture;

namespace QuestRoomMVC.WebMVC.Controllers
{
    public class SchedulesController : Controller
    {
        private readonly QuestRoomContext _context;

        public SchedulesController(QuestRoomContext context)
        {
            _context = context;
        }

        // GET: Schedules
        public async Task<IActionResult> Index(int weekOffset = 0, DateTime? selectedDate = null)
        {
            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday).AddDays(7 * weekOffset);
            var endOfWeek = startOfWeek.AddDays(6);

            selectedDate ??= startOfWeek;

            var schedules = await _context.Schedule
                .Include(s => s.Room)
                .ThenInclude(r => r.Location)
                .Where(s => s.StartTime.Date == selectedDate.Value.Date)
                .OrderBy(s => s.StartTime)
                .ToListAsync();

            ViewBag.WeekOffset = weekOffset;
            ViewBag.SelectedDate = selectedDate;
            return View(schedules);
        }
        public async Task<IActionResult> DetailsByRoom(int roomId, int weekOffset = 0, DateTime? selectedDate = null)
        {
            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday).AddDays(7 * weekOffset);
            var endOfWeek = startOfWeek.AddDays(6);

            selectedDate ??= startOfWeek;

            var schedules = await _context.Schedule
                .Include(s => s.Room)
                .ThenInclude(r => r.Location)
                .Where(s => s.Room.Id == roomId && s.StartTime.Date == selectedDate.Value.Date)
                .OrderBy(s => s.StartTime)
                .ToListAsync();

            var room = await _context.Room
                .Include(r => r.Location)
                .FirstOrDefaultAsync(r => r.Id == roomId);

            if (room == null)
                return NotFound();

            ViewBag.Room = room;
            ViewBag.WeekOffset = weekOffset;
            ViewBag.SelectedDate = selectedDate;
            return View("DetailsByRoom", schedules);
        }


        // GET: Schedules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedule
                .Include(s => s.Room)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (schedule == null)
            {
                return NotFound();
            }

            return View(schedule);
        }

        // GET: Schedules/Create
        public IActionResult Create()
        {
            ViewData["RoomId"] = new SelectList(_context.Room, "Id", "Name");
            return View();
        }

        // POST: Schedules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RoomId,StartTime,EndTime,IsBooked,Price,CreatedAt,Id")] Schedule schedule)
        {
            if (ModelState.IsValid)
            {
                _context.Add(schedule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoomId"] = new SelectList(_context.Room, "Id", "Name", schedule.RoomId);
            return View(schedule);
        }

        // GET: Schedules/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedule.FindAsync(id);
            if (schedule == null)
            {
                return NotFound();
            }
            ViewData["RoomId"] = new SelectList(_context.Room, "Id", "Name", schedule.RoomId);
            return View(schedule);
        }

        // POST: Schedules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RoomId,StartTime,EndTime,IsBooked,Price,CreatedAt,Id")] Schedule schedule)
        {
            if (id != schedule.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(schedule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScheduleExists(schedule.Id))
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
            ViewData["RoomId"] = new SelectList(_context.Room, "Id", "Name", schedule.RoomId);
            return View(schedule);
        }

        // GET: Schedules/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedule
                .Include(s => s.Room)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (schedule == null)
            {
                return NotFound();
            }

            return View(schedule);
        }

        // POST: Schedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var schedule = await _context.Schedule.FindAsync(id);
            if (schedule != null)
            {
                _context.Schedule.Remove(schedule);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ScheduleExists(int id)
        {
            return _context.Schedule.Any(e => e.Id == id);
        }
    }
}
