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
            IQueryable<Room> query = _context.Room
                 .Include(r => r.Genres)
                 .Include(r => r.Location)
                 .Include(r => r.Ratings);

            if (genreId != null)
            {
                query = query.Where(r => r.Genres.Any(g => g.Id == genreId));
                ViewBag.FilterType = "Genre";
                ViewBag.FilterId = genreId;
                ViewBag.FilterName = await _context.Genre
                    .Where(g => g.Id == genreId)
                    .Select(g => g.Name)
                    .FirstOrDefaultAsync();
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
            var rooms = await query.ToListAsync();
            ViewBag.Name = name;
            ViewBag.Genres = _context.Genre.Select(g => g.Name).ToList();
            ViewBag.Locations = _context.Location.Select(l => l.Name).ToList();
            return View(rooms);

        }
        public IActionResult Filter(List<int> locations, List<int> genres, int? maxPrice, int? players, int? difficulty)
        {
            var query = _context.Room
                .Include(r => r.Location)
                .Include(r => r.Genres)
                .AsQueryable();

            // Filter by Location
            if (locations != null && locations.Any())
            {
                query = query.Where(r => locations.Contains(r.LocationId));
            }

            // Filter by Genres (many-to-many relationship)
            if (genres != null && genres.Any())
            {
                query = query.Where(r => r.Genres.Any(g => genres.Contains(g.Id)));
            }

            // Filter by Players
            if (players.HasValue)
            {
                query = query.Where(r => r.MaxPlayers >= players.Value);
            }

            // Filter by Difficulty
            if (difficulty.HasValue)
            {
                query = query.Where(r => r.Difficulty == difficulty.Value);
            }

            // Return the filtered result
            var filteredRooms = query.ToList();
            return View("Index", filteredRooms);
        }

        // GET: Rooms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Room
                .Include(r => r.Genres)
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
            ViewBag.Genres = _context.Genre.Select(g => new { g.Id, g.Name }).ToList();
            ViewBag.Locations = _context.Location.Select(l => new { l.Id, l.Name }).ToList();
            return View();
        }

        // POST: Rooms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,LocationId,GenreId,MaxPlayers,Difficulty,Description,Image,Id")] Room room, int[] GenreIds)
        {
            if (ModelState.IsValid)
            {
                room.CreatedAt = DateTime.Now;
                room.UpdatedAt = DateTime.Now;
                room.Genres = new List<Genre>();
                if (GenreIds != null)
                {
                    foreach (var genreId in GenreIds)
                    {
                        var genre = await _context.Genre.FindAsync(genreId);
                        if (genre != null)
                        {
                            room.Genres.Add(genre);
                        }
                    }
                }

                _context.Add(room);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Genres"] = new SelectList(_context.Genre, "Id", "Name");
            ViewData["LocationId"] = new SelectList(_context.Location, "Id", "Name", room.LocationId);
            return View(room);
        }
        /*[HttpPost]
        [ActionName("CreateByGenre")]
        public async Task<IActionResult> Create(int[] GenreIds, [Bind("Name,LocationId,GenreId,MaxPlayers,Difficulty,Description,Image,Id")] Room room)
        {
            //room.GenreId = genreId;
            if (ModelState.IsValid)
            {
                room.CreatedAt = DateTime.Now;
                room.UpdatedAt = DateTime.Now;
                room.Genres = new List<Genre>();
                foreach (var genreId in GenreIds)
                {
                    var genre = await _context.Genre.FindAsync(genreId);
                    if (genre != null)
                    {
                        room.Genres.Add(genre);
                    }
                }
                _context.Add(room);
                await _context.SaveChangesAsync();
                //return RedirectToAction("Index", "Rooms", new { id = genreId, name = _context.Genre.Where(g => g.Id == genreId).FirstOrDefault().Name });
                return RedirectToAction(nameof(Index));
            }
            ViewData["Genres"] = new SelectList(_context.Genre, "Id", "Name");
            ViewData["LocationId"] = new SelectList(_context.Location, "Id", "Name", room.LocationId);
            return View(room);
            //return RedirectToAction( "Index", "Rooms", new {id = genreId, name = _context.Genre.Where(g => g.Id == genreId).FirstOrDefault().Name });
        }*/

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
            /*ViewData["GenreId"] = new SelectList(_context.Genre, "Id", "Name", room.GenreId);*/
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
                    /*existingRoom.GenreId = room.GenreId;*/
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
            /*ViewData["GenreId"] = new SelectList(_context.Genre, "Id", "Name", room.GenreId);*/
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
                /*.Include(r => r.Genre)*/
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
