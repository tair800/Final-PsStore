using Final.Mvc.ViewModels.WishlistVMs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;

namespace Final.Mvc.Controllers
{
    public class WishlistController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public WishlistController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "User");
            }

            var client = _httpClientFactory.CreateClient();
            HttpResponseMessage response = await client.GetAsync($"https://localhost:7047/api/Wishlist/{userId}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var wishlist = JsonConvert.DeserializeObject<WishlistVM>(data);
                return View(wishlist);
            }

            return View("Error", new { Message = "Could not fetch wishlist." });
        }
        public async Task<IActionResult> Get(string userId)
        {
            var client = _httpClientFactory.CreateClient();
            HttpResponseMessage response = await client.GetAsync($"https://localhost:7047/api/Wishlist/{userId}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var wishlist = JsonConvert.DeserializeObject<WishlistVM>(data);
                return View(wishlist);
            }

            return View("Error", new { Message = "Could not fetch wishlist." });
        }

        [HttpPost]
        public async Task<IActionResult> Add(string userId, int gameId)
        {
            var client = _httpClientFactory.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(new { userId, gameId }), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync($"https://localhost:7047/api/Wishlist/{userId}/add", content);

            if (response.IsSuccessStatusCode)
                return Ok();


            return BadRequest("Could not add game to wishlist.");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string userId, int gameId)
        {
            var client = _httpClientFactory.CreateClient();
            HttpResponseMessage response = await client.DeleteAsync($"https://localhost:7047/api/Wishlist/{userId}/delete/{gameId}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Get", new { userId });
            }

            return View("Error", new { Message = "Could not remove game from wishlist." });
        }
    }
}
