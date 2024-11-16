using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Up_To_Date__UTD_.Models;
using Serilog; 

namespace Up_To_Date__UTD_.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            Log.Information("HomeController initialized.");
        }

        public IActionResult Index()
        {
            Log.Information("Accessed the Home page.");
            return View();
        }

        public IActionResult Privacy()
        {
            Log.Information("Accessed the Privacy page.");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            Log.Error("An error occurred with RequestId: {RequestId}.", requestId);
            return View(new ErrorViewModel { RequestId = requestId });
        }
    }
}
