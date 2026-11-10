using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using RaceDay.Web.Models;

namespace RaceDay.Web.Controllers
{
    public class CategoriesController : ApiControllerBase
    {
        public CategoriesController(IHttpClientFactory factory) : base(factory) { }

        public async Task<IActionResult> Index(int eventId)
        {
            if (!IsLoggedIn() || !HasRole("Organiser"))
                return RedirectToAction("Login", "Account");

            var client = CreateApiClient();
            var eventItem = await client.GetFromJsonAsync<EventViewModels>($"api/events/{eventId}");
            if (eventItem == null) return NotFound();

            var categories = await client.GetFromJsonAsync<List<EventCategoryViewModels>>(
                $"api/events/{eventId}/categories") ?? new List<EventCategoryViewModels>();

            ViewBag.EventItem = eventItem;
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create(int eventId)
        {
            if (!IsLoggedIn() || !HasRole("Organiser"))
                return RedirectToAction("Login", "Account");

            return View(new EventCategoryViewModels { EventID = eventId, Gender = "All" });
        }

        [HttpPost]
        public async Task<IActionResult> Create(EventCategoryViewModels model)
        {
            if (!IsLoggedIn() || !HasRole("Organiser"))
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid) return View(model);

            var client = CreateApiClient();
            var response = await client.PostAsJsonAsync(
                $"api/events/{model.EventID}/categories", model);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", await response.Content.ReadAsStringAsync());
                return View(model);
            }

            TempData["Success"] = "Category added successfully.";
            return RedirectToAction(nameof(Index), new { eventId = model.EventID });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int eventId, int id)
        {
            if (!IsLoggedIn() || !HasRole("Organiser"))
                return RedirectToAction("Login", "Account");

            var client = CreateApiClient();
            var categories = await client.GetFromJsonAsync<List<EventCategoryViewModels>>(
                $"api/events/{eventId}/categories") ?? new List<EventCategoryViewModels>();
            var model = categories.FirstOrDefault(c => c.EventCategoryID == id);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int eventId, int id, EventCategoryViewModels model)
        {
            if (!IsLoggedIn() || !HasRole("Organiser"))
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid) return View(model);

            var client = CreateApiClient();
            var response = await client.PutAsJsonAsync(
                $"api/events/{eventId}/categories/{id}", model);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", await response.Content.ReadAsStringAsync());
                return View(model);
            }

            TempData["Success"] = "Category updated successfully.";
            return RedirectToAction(nameof(Index), new { eventId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int eventId, int id)
        {
            if (!IsLoggedIn() || !HasRole("Organiser"))
                return RedirectToAction("Login", "Account");

            var client = CreateApiClient();
            var response = await client.DeleteAsync(
                $"api/events/{eventId}/categories/{id}");

            TempData[response.IsSuccessStatusCode ? "Success" : "Error"] =
                response.IsSuccessStatusCode ? "Category deleted." : await response.Content.ReadAsStringAsync();

            return RedirectToAction(nameof(Index), new { eventId });
        }

    }
}
