using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace SampleWebApp.Controllers
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
            try
            {
                var x = 0;
                var y = 3;
                var z = y / x;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Empty);
            }

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            throw new Exception("A Test Exception");
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            var identifier = HttpContext.TraceIdentifier;

            // try get logId
            HttpContext.Items.TryGetValue("ExceptionLogId", out object logId);
            

            return View("Error", (identifier, logId?.ToString()));
        }
    }
}
