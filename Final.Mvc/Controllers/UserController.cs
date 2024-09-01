using Microsoft.AspNetCore.Mvc;

namespace Final.Mvc.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }

    }
}
