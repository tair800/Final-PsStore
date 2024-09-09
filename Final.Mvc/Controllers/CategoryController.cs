using Microsoft.AspNetCore.Mvc;

namespace Final.Mvc.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
