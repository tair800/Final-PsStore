using Final.Mvc.ViewModels.UserVMs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
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

                // Store the token securely
                Response.Cookies.Append("token", result.Token, new CookieOptions
                {
                    HttpOnly = false, // Ensure the token is only accessible via HTTP(S)
                    Secure = true,   // Secure the cookie (HTTPS)
                    SameSite = SameSiteMode.Strict, // Ensure the token is sent with same-site requests only
                    Expires = DateTimeOffset.Now.AddHours(1) // Optional: set an expiration time
                });

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
            return View();
        }

        // Send reset password email
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            var content = new StringContent(JsonConvert.SerializeObject(new { email = model.Email }), System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("https://localhost:7047/api/User/forgotPassword", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Reset link has been sent to your email.";
                return RedirectToAction("Login", "User");
            }
            else
            {
                ModelState.AddModelError("", "Failed to send reset link.");
                return View(model);
            }
        }

        // Reset password view
        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            return View(new ResetPasswordVM { Email = email, Token = token });
        }

        // Reset password action
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            var content = new StringContent(JsonConvert.SerializeObject(new { email = model.Email, token = model.Token, newPassword = model.NewPassword }), System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("https://localhost:7047/api/User/resetPassword", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Password has been reset successfully.";
                return RedirectToAction("Login", "User");
            }
            else
            {
                ModelState.AddModelError("", "Failed to reset password.");
                return View(model);
            }
        }
        public class UserToken
        {
            public string Token { get; set; }
        }
    }
}
