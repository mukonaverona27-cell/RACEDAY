using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Models;
using System.Diagnostics;

namespace RaceDay.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Check if the user is logged in
            var userID = HttpContext.Session.GetInt32("UserID");

            if (userID == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }
        public IActionResult Privacy()
        {
            // Only logged-in users can access this page
            var userID = HttpContext.Session.GetInt32("UserID");

            if (userID == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
