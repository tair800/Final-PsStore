using Final.Mvc.Areas.AdminArea.ViewModels.UserVMs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace YourProjectNamespace.Areas.Admin.Controllers
{
    [Area("AdminArea")]
    public class UserController : Controller
    {
        public async Task<IActionResult> Index()
        {
            List<UserListVM> users = new List<UserListVM>();

            using HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            HttpResponseMessage response = await client.GetAsync("https://localhost:7047/api/user/profiles");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                users = JsonConvert.DeserializeObject<List<UserListVM>>(data);
            }
            else
            {
                ViewBag.ErrorMessage = "Unable to retrieve user data.";
            }

            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(string id)
        {
            using HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            HttpResponseMessage response = await client.PostAsync($"https://localhost:7047/api/user/changeStatus/{id}", null);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "User status has been changed successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to change the user's status.";
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Detail(string id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            UserReturnVM user;

            using HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            HttpResponseMessage response = await client.GetAsync($"https://localhost:7047/api/user/profile/{id}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                user = JsonConvert.DeserializeObject<UserReturnVM>(data);
                return View(user);
            }
            else
            {
                return NotFound("User not found.");
            }
        }
    }
}
