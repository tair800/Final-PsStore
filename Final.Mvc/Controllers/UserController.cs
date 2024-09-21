using Final.Mvc.ViewModels.UserVMs;
using Microsoft.AspNetCore.Authorization;
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
            StringContent content = new(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("https://localhost:7047/api/user/login", content);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<UserToken>(data);

                Response.Cookies.Append("token", result.Token, new CookieOptions { HttpOnly = true, Secure = true });

                return RedirectToAction("Index", "Home");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ViewBag.ErrorMessage = $"Login failed: {errorContent}";
            }

            return View();
        }

        public IActionResult Register()
        {
            return View(new RegisterVM());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View(registerVM);
            }

            using HttpClient client = new();
            StringContent content = new StringContent(JsonConvert.SerializeObject(registerVM), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("https://localhost:7047/api/user/register", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login");
            }
            else
            {
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

        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordVM());
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using HttpClient client = new();
            StringContent content = new(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("https://localhost:7047/api/user/forgotPassword", content);

            if (response.IsSuccessStatusCode)
            {
                ViewBag.SuccessMessage = "Password reset link has been sent to your email.";
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"Failed to send password reset email: {errorContent}");
            }

            return View();
        }


        public IActionResult ResetPassword()
        {

            return View(new ResetPasswordVM());  // Ensure you're passing ResetPasswordVM to the view.
        }
        [Authorize(Roles = "member")]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            StringContent content = new(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("https://localhost:7047/api/user/resetPassword", content);

            if (response.IsSuccessStatusCode)
            {
                ViewBag.SuccessMessage = "Password has been successfully reset.";
                return RedirectToAction("Login", "User");
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", $"Failed to reset password: {errorContent}");

            return View(model);
        }


        public class UserToken
        {
            public string Token { get; set; }
        }
    }
}
