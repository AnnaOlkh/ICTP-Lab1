using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using QuestRoomMVC.Domain.Entities;
using QuestRoomMVC.Infrastracture;

namespace QuestRoomMVC.WebMVC.Controllers
{
    public class RoomsController : Controller
    {
        private readonly QuestRoomContext _context;

        public RoomsController(QuestRoomContext context)
        {
            _context = context;
        }

        // GET: Rooms
        public async Task<IActionResult> Index(int? genreId, int? locationId, string? name)
        {
            IQueryable<Room> query = _context.Room.Include(r => r.Genre).Include(r => r.Location);

            if (genreId != null)
            {
                query = query.Where(r => r.GenreId == genreId);
                ViewBag.FilterType = "Genre";
                ViewBag.FilterId = genreId;
                ViewBag.FilterName = _context.Genre.Where(g => g.Id == genreId).Select(g => g.Name).FirstOrDefault();
            }
            else if (locationId != null)
            {
                query = query.Where(r => r.LocationId == locationId);
                ViewBag.FilterType = "Location";
                ViewBag.FilterId = locationId;
                ViewBag.FilterName = _context.Location.Where(l => l.Id == locationId).Select(l => l.Name).FirstOrDefault();
            }
            else
            {
                // Додайте логіку за замовчуванням, наприклад, повернення всіх кімнат
                query = _context.Room;
            }

            ViewBag.Name = name;
            return View(await query.ToListAsync());

        }

        // GET: Rooms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Room
                .Include(r => r.Genre)
                .Include(r => r.Location)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        // GET: Rooms/Create
        public IActionResult Create()
        {
            ViewData["GenreId"] = new SelectList(_context.Genre, "Id", "Name");
            ViewData["LocationId"] = new SelectList(_context.Location, "Id", "Name");
            return View();
        }

        // POST: Rooms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,LocationId,GenreId,MaxPlayers,Difficulty,Description,Image,Id")] Room room)
        {
            if (ModelState.IsValid)
            {
                room.CreatedAt = DateTime.Now;
                room.UpdatedAt = DateTime.Now;
                _context.Add(room);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GenreId"] = new SelectList(_context.Genre, "Id", "Name", room.GenreId);
            ViewData["LocationId"] = new SelectList(_context.Location, "Id", "Name", room.LocationId);
            return View(room);
        }
        [HttpPost]
        [ActionName("CreateByGenre")]
        public async Task<IActionResult> Create(int genreId, [Bind("Name,LocationId,GenreId,MaxPlayers,Difficulty,Description,Image,Id")] Room room)
        {
            room.GenreId = genreId;
            if (ModelState.IsValid)
            {
                room.CreatedAt = DateTime.Now;
                room.UpdatedAt = DateTime.Now;
                _context.Add(room);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Rooms", new { id = genreId, name = _context.Genre.Where(g => g.Id == genreId).FirstOrDefault().Name });
            }
            //ViewData["GenreId"] = new SelectList(_context.Genre, "Id", "Name", room.GenreId);
            //ViewData["LocationId"] = new SelectList(_context.Location, "Id", "Name", room.LocationId);
            //return View(room);
            return RedirectToAction( "Index", "Rooms", new {id = genreId, name = _context.Genre.Where(g => g.Id == genreId).FirstOrDefault().Name });
        }

        // GET: Rooms/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Room.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }
            ViewData["GenreId"] = new SelectList(_context.Genre, "Id", "Name", room.GenreId);
            ViewData["LocationId"] = new SelectList(_context.Location, "Id", "Name", room.LocationId);
            return View(room);
        }

        // POST: Rooms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,LocationId,GenreId,MaxPlayers,Difficulty,Description,Image,Id")] Room room)
        {
            if (id != room.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingRoom = await _context.Room.FindAsync(id);
                    if (existingRoom == null)
                    {
                        return NotFound();
                    }

                    existingRoom.Name = room.Name;
                    existingRoom.LocationId = room.LocationId;
                    existingRoom.GenreId = room.GenreId;
                    existingRoom.MaxPlayers = room.MaxPlayers;
                    existingRoom.Difficulty = room.Difficulty;
                    existingRoom.Description = room.Description;
                    existingRoom.Image = room.Image;
                    existingRoom.UpdatedAt = DateTime.Now;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomExists(room.Id))
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
            ViewData["GenreId"] = new SelectList(_context.Genre, "Id", "Name", room.GenreId);
            ViewData["LocationId"] = new SelectList(_context.Location, "Id", "Name", room.LocationId);
            return View(room);
        }

        // GET: Rooms/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Room
                .Include(r => r.Genre)
                .Include(r => r.Location)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }

        // POST: Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Room.FindAsync(id);
            if (room != null)
            {
                _context.Room.Remove(room);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoomExists(int id)
        {
            return _context.Room.Any(e => e.Id == id);
        }
    }
}
