using Microsoft.AspNetCore.Mvc;

namespace EngageTrack.Controllers
{
    public class StudentProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult EditProfile()
        {
            return View();
        }
        public IActionResult ViewProfile()
        {
            return View();
        }
    }
}
