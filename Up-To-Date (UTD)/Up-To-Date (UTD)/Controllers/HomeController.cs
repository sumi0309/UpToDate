using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Up_To_Date__UTD_.Models;

namespace Up_To_Date__UTD_.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        // Constructor to initialize the logger for the HomeController.
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Displays the home page.
        public IActionResult Index()
        {
            return View();
        }

        // Displays the privacy policy page.
        public IActionResult Privacy()
        {
            return View();
        }

        // Returns the error view with relevant error information.
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
