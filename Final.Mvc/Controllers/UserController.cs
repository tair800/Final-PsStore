﻿using Final.Mvc.ViewModels.UserVMs;
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

                // Store the token securely
                Response.Cookies.Append("token", result.Token, new CookieOptions
                {
                    HttpOnly = true, // Ensure the token is only accessible via HTTP(S)
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
        public class UserToken
        {
            public string Token { get; set; }
        }
    }
}
