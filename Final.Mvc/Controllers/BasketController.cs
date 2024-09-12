using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace Final.Mvc.Controllers
{
    public class BasketController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BasketController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Get(string email)
        {
            var client = _httpClientFactory.CreateClient();
            HttpResponseMessage response = await client.GetAsync($"https://localhost:7047/api/Basket/{email}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var basket = JsonConvert.DeserializeObject<BasketListVM>(data);
                return View(basket);
            }

            return View("Error", new { Message = "Could not fetch basket." });
        }
        [HttpPost]
        public async Task<IActionResult> AddToBasket(string email, int gameId, int quantity)
        {
            var client = _httpClientFactory.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(new { email, gameId, quantity }), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync($"https://localhost:7047/api/Basket/add?email={email}&gameId={gameId}&quantity={quantity}", content);

            if (response.IsSuccessStatusCode)
            {
                var updatedBasket = await response.Content.ReadAsStringAsync();
                return RedirectToAction("GetBasket", new { email });
            }

            return View("Error", new { Message = "Could not add game to basket." });
        }

        [HttpPost]
        public async Task<IActionResult> Update(string email, int gameId, int quantity)
        {
            var client = _httpClientFactory.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(new { email, gameId, quantity }), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync($"https://localhost:7047/api/Basket/update?email={email}&gameId={gameId}&quantity={quantity}", content);

            if (response.IsSuccessStatusCode)
            {
                var updatedBasket = await response.Content.ReadAsStringAsync();
                return RedirectToAction("GetBasket", new { email });
            }

            return View("Error", new { Message = "Could not update basket." });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string email, int gameId)
        {
            var client = _httpClientFactory.CreateClient();
            HttpResponseMessage response = await client.DeleteAsync($"https://localhost:7047/api/Basket/remove?email={email}&gameId={gameId}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("GetBasket", new { email });
            }

            return View("Error", new { Message = "Could not remove game from basket." });
        }

        [HttpPost]
        public async Task<IActionResult> Clear(string email)
        {
            var client = _httpClientFactory.CreateClient();
            HttpResponseMessage response = await client.DeleteAsync($"https://localhost:7047/api/Basket/clear?email={email}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("GetBasket", new { email });
            }

            return View("Error", new { Message = "Could not clear basket." });
        }
    }
}
