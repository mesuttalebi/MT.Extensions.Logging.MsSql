using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SampleWebAppNetFramework.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger _logger;
        public HomeController (ILogger<HomeController> logger)
	    {
            _logger = logger;
	    }

        public IActionResult Index()
        {
            // Test Logger
            _logger.LogError("This is Critical Error");
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            throw new Exception("Hello Logging!");
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
