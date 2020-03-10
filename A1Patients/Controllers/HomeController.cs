using A1Patients.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace A1Patients.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            TempData["message"] = "Error";
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
