using Final.Mvc.ViewModels.GameVMs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Final.Mvc.Controllers
{
    public class GameController : Controller
    {
        public async Task<IActionResult> Index(string category = null, int? platform = null, string sortByPrice = null, string sortByDate = null)
        {
            using HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            HttpResponseMessage response = await client.GetAsync("https://localhost:7047/api/Game");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<List<GameListItemVM>>(data);

                // Apply filters on the game list
                if (!string.IsNullOrEmpty(category))
                {
                    result = result.Where(g => g.CategoryName == category).ToList();
                }

                if (platform.HasValue)
                {
                    result = result.Where(g => (int)g.Platform == platform).ToList();
                }

                // Sort by price
                switch (sortByPrice)
                {
                    case "price_asc":
                        result = result.OrderBy(g => g.Price).ToList();
                        break;
                    case "price_desc":
                        result = result.OrderByDescending(g => g.Price).ToList();
                        break;
                    default:
                        break;
                }

                // Sort by release date
                switch (sortByDate)
                {
                    case "date_asc":
                        result = result.OrderBy(g => g.CreatedDate).ToList();
                        break;
                    case "date_desc":
                        result = result.OrderByDescending(g => g.CreatedDate).ToList();
                        break;
                    default:
                        break;
                }

                return View(result);
            }

            return BadRequest("Error fetching games.");
        }


        public async Task<IActionResult> Detail(int id)
        {
            using HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            HttpResponseMessage response = await client.GetAsync($"https://localhost:7047/api/Game/Get/{id}");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<GameDetailVM>(data);
                return View(result);
            }
            return NotFound("Game not found.");
        }

        [HttpGet]
        public async Task<IActionResult> Search(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return Json(new List<GameListItemVM>());
            }

            using HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            HttpResponseMessage response = await client.GetAsync($"https://localhost:7047/api/Game/Search?title={title}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var searchResults = JsonConvert.DeserializeObject<List<GameListItemVM>>(data);

                // Return search results as JSON
                return Json(searchResults);
            }

            return Json(new List<GameListItemVM>());
        }
    }
}
