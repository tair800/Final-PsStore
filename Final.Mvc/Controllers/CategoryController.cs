using Final.Mvc.ViewModels.CategoryVMs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Final.Mvc.Controllers
{
    public class CategoryController : Controller
    {
        public async Task<IActionResult> Index()
        {
            // Initialize an empty list of CategoryFirstVM
            List<CategoryFirstVM> categories = new List<CategoryFirstVM>();

            using HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            // Make the GET request to the API to fetch all categories
            HttpResponseMessage response = await client.GetAsync("https://localhost:7047/api/Category");

            // If the response is successful, deserialize the data into the CategoryFirstVM list
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                categories = JsonConvert.DeserializeObject<List<CategoryFirstVM>>(data);
            }

            // Pass the list of categories to the view
            return View(categories);
        }

        public async Task<IActionResult> Detail(int id)
        {
            using HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            HttpResponseMessage response = await client.GetAsync($"https://localhost:7047/api/Category/{id}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<CategoryFirstVM>(data);
                return View(result);
            }

            return NotFound("Category not found.");
        }
    }
}
