using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using RaceDay.Web.Models;

namespace RaceDay.Web.Controllers
{
    public class EventsController : ApiControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public EventsController(IHttpClientFactory factory, IWebHostEnvironment environment)
            : base(factory)
        {
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            if (!IsLoggedIn() || !HasRole("Organiser"))
                return RedirectToAction("Login", "Account");

            var client = CreateApiClient();
            var allEvents = await client.GetFromJsonAsync<List<EventViewModels>>("api/events")
                            ?? new List<EventViewModels>();

            // The API event list includes organiser ownership. The current organiser's ID
            // is stored in the MVC session after login.
            var organiserId = HttpContext.Session.GetInt32("UserID");
            var myEvents = allEvents.Where(e => e.OrganiserID == organiserId).ToList();
            return View(myEvents);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!IsLoggedIn() || !HasRole("Organiser"))
                return RedirectToAction("Login", "Account");
            return View(new EventViewModels { EventDate = DateTime.Today });
        }

        [HttpPost]
        public async Task<IActionResult> Create(EventViewModels model, IFormFile? bannerImage)
        {
            if (!IsLoggedIn() || !HasRole("Organiser"))
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid) return View(model);

            if (bannerImage != null && bannerImage.Length > 0)
            {
                var savedPath = await SaveImage(bannerImage, "images/events");
                if (savedPath == null)
                {
                    ModelState.AddModelError("BannerImagePath", "Choose a JPG, PNG or WEBP image smaller than 4 MB.");
                    return View(model);
                }
                model.BannerImagePath = savedPath;
            }

            var client = CreateApiClient();
            var response = await client.PostAsJsonAsync("api/events", model);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", await response.Content.ReadAsStringAsync());
                return View(model);
            }

            TempData["Success"] = "Event created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsLoggedIn() || !HasRole("Organiser"))
                return RedirectToAction("Login", "Account");

            var client = CreateApiClient();
            var model = await client.GetFromJsonAsync<EventViewModels>($"api/events/{id}");
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EventViewModels model, IFormFile? bannerImage)
        {
            if (!IsLoggedIn() || !HasRole("Organiser"))
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid) return View(model);

            if (bannerImage != null && bannerImage.Length > 0)
            {
                var savedPath = await SaveImage(bannerImage, "images/events");
                if (savedPath == null)
                {
                    ModelState.AddModelError("BannerImagePath", "Choose a JPG, PNG or WEBP image smaller than 4 MB.");
                    return View(model);
                }
                model.BannerImagePath = savedPath;
            }

            var client = CreateApiClient();
            var response = await client.PutAsJsonAsync($"api/events/{id}", model);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", await response.Content.ReadAsStringAsync());
                return View(model);
            }

            TempData["Success"] = "Event updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsLoggedIn() || !HasRole("Organiser"))
                return RedirectToAction("Login", "Account");

            var client = CreateApiClient();
            var response = await client.DeleteAsync($"api/events/{id}");
            TempData[response.IsSuccessStatusCode ? "Success" : "Error"] =
                response.IsSuccessStatusCode ? "Event deleted." : await response.Content.ReadAsStringAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task<string?> SaveImage(IFormFile file, string folder)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            if (!allowed.Contains(extension) || file.Length > 4 * 1024 * 1024)
                return null;

            var directory = Path.Combine(_environment.WebRootPath, folder.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(directory);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(directory, fileName);
            await using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return "/" + folder.Trim('/') + "/" + fileName;
        }
    }
}
