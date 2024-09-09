﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Final.Mvc.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Detail(int id)
        {
            using HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            HttpResponseMessage response = await client.GetAsync($"https://localhost:7047/api/Category/{id}");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<CategoryDetailVM>(data);
                return View(result);
            }
            return NotFound("Category not found.");
        }
    }
}
