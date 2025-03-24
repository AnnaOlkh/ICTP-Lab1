using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestRoomMVC.Infrastracture;

namespace QuestRoomMVC.WebMVC.Controllers
{
    public class ChartsController : Controller
    {
        private readonly QuestRoomContext _context;

        public ChartsController(QuestRoomContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "admin")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
