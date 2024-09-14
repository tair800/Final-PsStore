using Final.Mvc.ViewModels.UserVMs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Final.Mvc.Controllers
{
    public class UserController : Controller
    {


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            using HttpClient client = new();
            StringContent content = new StringContent(JsonConvert.SerializeObject(new { model }), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("https://localhost:7047/api/user/login", content);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<UserToken>(data);

                Response.Cookies.Append("token", result.Token, new CookieOptions { HttpOnly = true, Secure = true });

                return RedirectToAction("Index", "Home");
            }

            ViewBag.ErrorMessage = "Invalid username or password.";
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            // Check if the model is valid (server-side validation)
            if (!ModelState.IsValid)
            {
                return View(registerVM);
            }

            using HttpClient client = new();
            StringContent content = new StringContent(JsonConvert.SerializeObject(registerVM), Encoding.UTF8, "application/json");

            // Send the registration data to the API
            HttpResponseMessage response = await client.PostAsync("https://localhost:7047/api/user/register", content);

            if (response.IsSuccessStatusCode)
            {
                // On successful registration, redirect to the login page
                return RedirectToAction("Login");
            }
            else
            {
                // If registration fails, get the error message from the API response
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"Registration failed: {errorContent}");
                return View(registerVM);
            }
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("token");

            return RedirectToAction("Login");
        }

        public class UserToken
        {
            public string Token { get; set; }
        }
    }
}
