using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using A7.Server.Models;
using System.Collections.Generic;

namespace A7.Server.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        static List<string> Comments = new List<string>();

        public HomeController(ILogger<HomeController> logger) => _logger = logger;

        public IActionResult Index() => View();

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    
        public IActionResult Contact()
        {
            ViewBag.Comments = Comments;
            return View();
        }

        [HttpPost]
        public ActionResult Contact(string comment)
        {
            Comments.Add(comment);
            ViewBag.Comments = Comments;
            return View();
        }
    }
}