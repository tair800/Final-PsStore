using Final.Mvc.ViewModels.GameVMs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Final.Mvc.Controllers
{
    public class GameController : Controller
    {
        public async Task<IActionResult> Index()
        {
            using HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            HttpResponseMessage response = await client.GetAsync("https://localhost:7047/api/Game?page=1");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<List<GameListItemVM>>(data);
                return View(result);
            }
            return BadRequest("error");
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
    }
}
