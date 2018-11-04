using Microsoft.AspNetCore.Mvc;

namespace FileProcessorTDXTechTest.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
