using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using RaceDay.Web.Models;

namespace RaceDay.Web.Controllers
{
    public class ProfileController : ApiControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public ProfileController(IHttpClientFactory factory, IWebHostEnvironment environment)
            : base(factory)
        {
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            if (!IsLoggedIn() || !HasRole("Participant"))
                return RedirectToAction("Login", "Account");

            var client = CreateApiClient();
            var profile = await client.GetFromJsonAsync<ProfileViewModels>("api/profile");
            if (profile == null) return NotFound();
            return View(profile);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            if (!IsLoggedIn() || !HasRole("Participant"))
                return RedirectToAction("Login", "Account");

            var client = CreateApiClient();
            var profile = await client.GetFromJsonAsync<ProfileViewModels>("api/profile");
            if (profile == null) return NotFound();
            return View(profile);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProfileViewModels model, IFormFile? profileImage)
        {
            if (!IsLoggedIn() || !HasRole("Participant"))
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid) return View(model);

            if (profileImage != null && profileImage.Length > 0)
            {
                var extension = Path.GetExtension(profileImage.FileName).ToLowerInvariant();
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };

                if (!allowed.Contains(extension) || profileImage.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError("ProfileImagePath", "Choose a JPG, PNG or WEBP image smaller than 2 MB.");
                    return View(model);
                }

                var folder = Path.Combine(_environment.WebRootPath, "images", "profiles");
                Directory.CreateDirectory(folder);

                var fileName = $"{Guid.NewGuid()}{extension}";
                var fullPath = Path.Combine(folder, fileName);

                await using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await profileImage.CopyToAsync(stream);
                }

                // Store only the wwwroot-relative path in the API database.
                model.ProfileImagePath = "/images/profiles/" + fileName;
            }

            model.ParticipantID = HttpContext.Session.GetInt32("UserID") ?? 0;
            var client = CreateApiClient();

            // Keep the current picture if the user did not upload a replacement.
            if (string.IsNullOrWhiteSpace(model.ProfileImagePath))
            {
                var currentProfile = await client.GetFromJsonAsync<ProfileViewModels>("api/profile");
                model.ProfileImagePath = currentProfile?.ProfileImagePath;
            }
            var response = await client.PutAsJsonAsync(
                $"api/participants/{model.ParticipantID}", model);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", await response.Content.ReadAsStringAsync());
                return View(model);
            }

            TempData["Success"] = "Your profile has been updated.";
            return RedirectToAction(nameof(Index));
        }
    }
}
