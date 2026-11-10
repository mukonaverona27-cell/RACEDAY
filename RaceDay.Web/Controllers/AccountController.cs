using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Models;
using System.Net.Http.Json;

namespace RaceDay.Web.Controllers
{
    public class AccountController : ApiControllerBase
    {
        public AccountController(IHttpClientFactory clientFactory) : base(clientFactory)
        {
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModels model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var client = CreateApiClient();
            var response = await client.PostAsJsonAsync("api/auth/login", model);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Invalid email, password or account type.");
                return View(model);
            }

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

            if (result == null)
            {
                ModelState.AddModelError("", "The API did not return login information.");
                return View(model);
            }

            // Save the API session cookie so later MVC requests can access protected API endpoints.
            if (response.Headers.TryGetValues("Set-Cookie", out var cookieHeaders))
            {
                var sessionCookie = cookieHeaders.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(sessionCookie))
                {
                    HttpContext.Session.SetString("ApiSessionCookie", sessionCookie.Split(';')[0]);
                }
            }

            HttpContext.Session.SetInt32("UserID", result.UserID);
            HttpContext.Session.SetString("Role", result.Role);
            HttpContext.Session.SetString("Email", result.Email);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModels model)
        {
            if (model.Role == "Participant")
            {
                if (string.IsNullOrWhiteSpace(model.Surname))
                    ModelState.AddModelError(nameof(model.Surname), "Surname is required for participants.");
                if (string.IsNullOrWhiteSpace(model.IDNumber))
                    ModelState.AddModelError(nameof(model.IDNumber), "ID number is required for participants.");
                if (model.DateOfBirth == default)
                    ModelState.AddModelError(nameof(model.DateOfBirth), "Date of birth is required for participants.");
                if (string.IsNullOrWhiteSpace(model.Gender))
                    ModelState.AddModelError(nameof(model.Gender), "Gender is required for participants.");
            }

            if (!ModelState.IsValid)
                return View(model);

            var client = CreateApiClient();
            var response = await client.PostAsJsonAsync("api/auth/register", model);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", string.IsNullOrWhiteSpace(error)
                    ? "Registration could not be completed."
                    : error);
                return View(model);
            }

            TempData["Success"] = "Your account has been created. Please log in.";
            return RedirectToAction(nameof(Login));
        }

        public async Task<IActionResult> Logout()
        {
            try
            {
                var client = CreateApiClient();
                await client.PostAsync("api/auth/logout", null);
            }
            catch
            {
                // The MVC session is still cleared if the API is unavailable.
            }

            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }

        private class LoginResponse
        {
            public int UserID { get; set; }
            public string Email { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty;
        }
    }
}
