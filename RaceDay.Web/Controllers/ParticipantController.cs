using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using RaceDay.Web.Models;

namespace RaceDay.Web.Controllers
{
    public class ParticipantController : ApiControllerBase
    {
        public ParticipantController(IHttpClientFactory factory) : base(factory) { }

        public async Task<IActionResult> Index()
        {
            if (!IsLoggedIn() || !HasRole("Participant"))
                return RedirectToAction("Login", "Account");

            var client = CreateApiClient();
            var events = await client.GetFromJsonAsync<List<EventViewModels>>("api/events")
                         ?? new List<EventViewModels>();
            var enrolments = await client.GetFromJsonAsync<List<EnrolmentViewModels>>("api/enrolments/my")
                             ?? new List<EnrolmentViewModels>();

            ViewBag.EventCount = events.Count;
            ViewBag.EnrolmentCount = enrolments.Count;
            return View();
        }

        public async Task<IActionResult> Events(string? search, string? eventType)
        {
            if (!IsLoggedIn() || !HasRole("Participant"))
                return RedirectToAction("Login", "Account");

            var client = CreateApiClient();
            var events = await client.GetFromJsonAsync<List<EventViewModels>>("api/events")
                         ?? new List<EventViewModels>();

            events = events.Where(e => e.EventDate.Date >= DateTime.Today).ToList();

            if (!string.IsNullOrWhiteSpace(search))
                events = events.Where(e =>
                    e.EventName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    e.City.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    e.Venue.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrWhiteSpace(eventType))
                events = events.Where(e => e.EventType.Equals(eventType, StringComparison.OrdinalIgnoreCase)).ToList();

            ViewBag.Search = search;
            ViewBag.EventType = eventType;
            return View(events.OrderBy(e => e.EventDate).ToList());
        }

        public async Task<IActionResult> Details(int id)
        {
            if (!IsLoggedIn() || !HasRole("Participant"))
                return RedirectToAction("Login", "Account");

            var client = CreateApiClient();
            var item = await client.GetFromJsonAsync<EventViewModels>($"api/events/{id}");
            if (item == null) return NotFound();

            var categories = await client.GetFromJsonAsync<List<EventCategoryViewModels>>(
                $"api/events/{id}/categories") ?? new List<EventCategoryViewModels>();

            ViewBag.Categories = categories;
            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> Enrol(int eventId, int eventCategoryId)
        {
            if (!IsLoggedIn() || !HasRole("Participant"))
                return RedirectToAction("Login", "Account");

            var client = CreateApiClient();
            var enrolment = new EnrolmentViewModels
            {
                EventID = eventId,
                EventCategoryID = eventCategoryId,
                Status = "Confirmed"
            };

            var response = await client.PostAsJsonAsync("api/enrolments", enrolment);
            if (!response.IsSuccessStatusCode)
                TempData["Error"] = await response.Content.ReadAsStringAsync();
            else
                TempData["Success"] = "You have successfully enrolled in this event.";

            return RedirectToAction(nameof(Details), new { id = eventId });
        }

        public async Task<IActionResult> MyEvents()
        {
            if (!IsLoggedIn() || !HasRole("Participant"))
                return RedirectToAction("Login", "Account");

            var client = CreateApiClient();
            var items = await client.GetFromJsonAsync<List<EnrolmentViewModels>>("api/enrolments/my")
                        ?? new List<EnrolmentViewModels>();
            return View(items);
        }

        public async Task<IActionResult> MyResults()
        {
            if (!IsLoggedIn() || !HasRole("Participant"))
                return RedirectToAction("Login", "Account");

            var client = CreateApiClient();
            var items = await client.GetFromJsonAsync<List<ResultViewModels>>("api/results/my")
                        ?? new List<ResultViewModels>();
            return View(items);
        }
    }
}
