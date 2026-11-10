using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using RaceDay.Web.Models;

namespace RaceDay.Web.Controllers
{
    public class ResultsController : ApiControllerBase
    {
        public ResultsController(IHttpClientFactory factory) : base(factory) { }

        public async Task<IActionResult> Index(int eventId)
        {
            if (!IsLoggedIn() || !HasRole("Organiser"))
                return RedirectToAction("Login", "Account");

            var client = CreateApiClient();
            var results = await client.GetFromJsonAsync<List<ResultViewModels>>(
                $"api/results/event/{eventId}") ?? new List<ResultViewModels>();

            ViewBag.EventID = eventId;
            return View(results);
        }

        [HttpGet]
        public IActionResult Create(int eventId, int enrolmentId)
        {
            if (!IsLoggedIn() || !HasRole("Organiser"))
                return RedirectToAction("Login", "Account");

            return View(new ResultViewModels { EventID = eventId, EnrolmentID = enrolmentId });
        }

        [HttpPost]
        public async Task<IActionResult> Create(int eventId, ResultViewModels model)
        {
            if (!IsLoggedIn() || !HasRole("Organiser"))
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid) return View(model);

            var client = CreateApiClient();
            var response = await client.PostAsJsonAsync("api/results", model);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", await response.Content.ReadAsStringAsync());
                return View(model);
            }

            TempData["Success"] = "Result saved successfully.";
            return RedirectToAction(nameof(Index), new { eventId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, int eventId)
        {
            if (!IsLoggedIn() || !HasRole("Organiser"))
                return RedirectToAction("Login", "Account");

            var client = CreateApiClient();
            var response = await client.DeleteAsync($"api/results/{id}");
            TempData[response.IsSuccessStatusCode ? "Success" : "Error"] =
                response.IsSuccessStatusCode ? "Result deleted." : await response.Content.ReadAsStringAsync();

            return RedirectToAction(nameof(Index), new { eventId });
        }
    }
}
