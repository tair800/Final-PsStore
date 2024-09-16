using Final.Mvc.Areas.AdminArea.ViewModels.Categories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Final.Mvc.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class CategoryController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CategoryController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Index action to list games
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            var response = await client.GetAsync("https://localhost:7047/api/Category");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var categories = JsonConvert.DeserializeObject<List<CategoryListVM>>(data);
                return View(categories);
            }

            return View(new List<CategoryListVM>());
        }

        public async Task<IActionResult> Detail(int id)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            var response = await client.GetAsync($"https://localhost:7047/api/category/{id}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var categories = JsonConvert.DeserializeObject<AdminCategoryReturn>(data);
                return View(categories);
            }

            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            var response = await client.DeleteAsync($"https://localhost:7047/api/Category/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Error deleting the category.");
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new AdminCategoryCreateVM());
        }

        [HttpPost]
        public async Task<IActionResult> Create(AdminCategoryCreateVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            var jsonContent = new StringContent(JsonConvert.SerializeObject(model), System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://localhost:7047/api/Category", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Error creating category.");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            var response = await client.GetAsync($"https://localhost:7047/api/Category/{id}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var category = JsonConvert.DeserializeObject<AdminCategoryUpdateVM>(data);
                return View(category);
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, AdminCategoryUpdateVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            var jsonContent = new StringContent(JsonConvert.SerializeObject(model), System.Text.Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"https://localhost:7047/api/Category/{id}", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Error updating category.");
            return View(model);
        }



    }
}
