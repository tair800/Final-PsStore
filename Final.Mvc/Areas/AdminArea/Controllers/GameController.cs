using Final.Mvc.Areas.AdminArea.ViewModels.GameVMs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Final.Mvc.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class GameController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public GameController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            var response = await client.GetAsync("https://localhost:7047/api/Game");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var games = JsonConvert.DeserializeObject<List<GameListVM>>(data);
                return View(games);
            }

            return View(new List<GameListVM>());
        }

        public async Task<IActionResult> Detail(int id)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            var response = await client.GetAsync($"https://localhost:7047/api/Game/Get/{id}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var game = JsonConvert.DeserializeObject<AdminGameReturnVM>(data);
                return View(game);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            var response = await client.GetAsync($"https://localhost:7047/api/Game/Get/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            var data = await response.Content.ReadAsStringAsync();
            var game = JsonConvert.DeserializeObject<AdminGameUpdateVM>(data);

            // Fetch the categories from the API
            var categoryResponse = await client.GetAsync("https://localhost:7047/api/Category");
            if (categoryResponse.IsSuccessStatusCode)
            {
                var categoryData = await categoryResponse.Content.ReadAsStringAsync();
                game.Categories = JsonConvert.DeserializeObject<List<CategoryVM>>(categoryData);
            }

            return View(game);
        }


        [HttpPost]
        public async Task<IActionResult> Update(AdminGameUpdateVM model)
        {
            if (!ModelState.IsValid)
            {
                var clients = _httpClientFactory.CreateClient();
                var categoryResponse = await clients.GetAsync("https://localhost:7047/api/Category");
                if (categoryResponse.IsSuccessStatusCode)
                {
                    var categoryData = await categoryResponse.Content.ReadAsStringAsync();
                    model.Categories = JsonConvert.DeserializeObject<List<CategoryVM>>(categoryData);
                }

                return View(model);
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            var formContent = new MultipartFormDataContent
    {
        { new StringContent(model.Title ?? ""), "Title" },
        { new StringContent(model.Description ?? ""), "Description" },
        { new StringContent(model.Price?.ToString() ?? "0"), "Price" },
        { new StringContent(model.SalePrice?.ToString() ?? "0"), "SalePrice" },
        { new StringContent(model.CategoryId.ToString()), "CategoryId" },
        { new StringContent(model.Platform?.ToString() ?? ""), "Platform" }
    };

            if (model.File != null)
            {
                var fileContent = new StreamContent(model.File.OpenReadStream());
                formContent.Add(fileContent, "File", model.File.FileName);
            }

            var response = await client.PutAsync($"https://localhost:7047/api/Game/{model.Id}", formContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            var response = await client.DeleteAsync($"https://localhost:7047/api/Game/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Error deleting the game.");
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new AdminGameCreateVM();

            // Fetch the categories from the API to populate the dropdown
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            var categoryResponse = await client.GetAsync("https://localhost:7047/api/Category");
            if (categoryResponse.IsSuccessStatusCode)
            {
                var categoryData = await categoryResponse.Content.ReadAsStringAsync();
                model.Categories = JsonConvert.DeserializeObject<List<AdminCategoryVM>>(categoryData);
            }
            else
            {
                model.Categories = new List<AdminCategoryVM>();
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AdminGameCreateVM model)
        {
            if (!ModelState.IsValid)
            {
                var clients = _httpClientFactory.CreateClient();
                var categoryResponse = await clients.GetAsync("https://localhost:7047/api/Category");
                if (categoryResponse.IsSuccessStatusCode)
                {
                    var categoryData = await categoryResponse.Content.ReadAsStringAsync();
                    model.Categories = JsonConvert.DeserializeObject<List<AdminCategoryVM>>(categoryData);
                }
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            var formContent = new MultipartFormDataContent
            {
                { new StringContent(model.Title ?? ""), "Title" },
                { new StringContent(model.Description ?? ""), "Description" },
                { new StringContent(model.Price.ToString()), "Price" },
                { new StringContent(model.SalePrice?.ToString() ?? "0"), "SalePrice" },
                { new StringContent(model.CategoryId.ToString()), "CategoryId" },
                { new StringContent(model.Platform.ToString()), "Platform" }
            };

            if (model.ImgUrl != null)
            {
                var fileContent = new StreamContent(model.ImgUrl.OpenReadStream());
                formContent.Add(fileContent, "ImgUrl", model.ImgUrl.FileName);
            }

            var response = await client.PostAsync("https://localhost:7047/api/Game", formContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "There was an error creating the game.");
            return View(model);
        }


    }
}
