using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using RaceDay.Web.Models;

namespace RaceDay.Web.Controllers
{
    public class EnrolmentsController : ApiControllerBase
    {
        public EnrolmentsController(IHttpClientFactory factory) : base(factory) { }

        public async Task<IActionResult> Index(int eventId)
        {
            if (!IsLoggedIn() || !HasRole("Organiser"))
                return RedirectToAction("Login", "Account");

            var client = CreateApiClient();
            var eventItem = await client.GetFromJsonAsync<EventViewModels>($"api/events/{eventId}");
            if (eventItem == null) return NotFound();

            var enrolments = await client.GetFromJsonAsync<List<EnrolmentViewModels>>(
                $"api/enrolments/event/{eventId}") ?? new List<EnrolmentViewModels>();

            ViewBag.EventItem = eventItem;
            return View(enrolments);
        }
    }
}
