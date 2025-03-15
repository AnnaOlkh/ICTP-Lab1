using Microsoft.AspNetCore.Mvc;
using QuestRoomMVC.Domain.Entities;

namespace QuestRoomMVC.WebMVC.Components;

public class SidebarViewComponent : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(IEnumerable<string> locations, IEnumerable<string> genres)
    {
        ViewBag.Locations = locations;
        ViewBag.Genres = genres;

        return View();
    }
}
