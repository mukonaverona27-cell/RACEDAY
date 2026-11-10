using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using RaceDay.Web.Models;

namespace RaceDay.Web.Controllers
{
    public class OrganiserController : ApiControllerBase
    {
        public OrganiserController(IHttpClientFactory factory) : base(factory) { }

        public async Task<IActionResult> Index()
        {
            if (!IsLoggedIn() || !HasRole("Organiser"))
                return RedirectToAction("Login", "Account");

            var client = CreateApiClient();
            var events = await client.GetFromJsonAsync<List<EventViewModels>>("api/events")
                         ?? new List<EventViewModels>();

            var organiserId = HttpContext.Session.GetInt32("UserID");
            var myEvents = events.Where(e => e.OrganiserID == organiserId).ToList();
            ViewBag.EventCount = myEvents.Count;
            return View(myEvents);
        }
    }
}
