using Final.Mvc.ViewModels.BasketVMs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Final.Mvc.Controllers
{
    public class BasketController : Controller
    {
        private readonly HttpClient _client;

        public BasketController(HttpClient client)
        {
            _client = client;
        }

        public async Task<IActionResult> GetBasket()
        {
            var token = Request.Cookies["token"];
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("No token found in cookies.");

                return PartialView("_BasketPartial", null);
            }

            using HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await client.GetAsync("https://localhost:7047/api/Basket/get/{userId}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var basket = JsonConvert.DeserializeObject<List<BasketItemVM>>(data); // Ensure this deserialization step is correct
                return PartialView("_BasketPartial", basket); // Ensure you're passing the correct data to the view
            }
            else
            {
                // Log for debugging
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error fetching basket: {error}");

                return PartialView("_BasketPartial", null);
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddToBasket(int gameId, int quantity)
        {
            // Check if the user is logged in
            var email = Request.Cookies["userEmail"];
            var token = Request.Cookies["token"];

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                // If the user is not logged in, redirect to the login page
                return RedirectToAction("Login", "User");
            }

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.PostAsync($"https://localhost:7047/api/Basket/add?email={email}&gameId={gameId}&quantity={quantity}", null);

            if (response.IsSuccessStatusCode)
            {
                return Ok();
            }

            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromBasket(int gameId)
        {
            // Check if the user is logged in
            var email = Request.Cookies["userEmail"];
            var token = Request.Cookies["token"];

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                // If the user is not logged in, redirect to the login page
                return RedirectToAction("Login", "User");
            }

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.DeleteAsync($"https://localhost:7047/api/Basket/remove?email={email}&gameId={gameId}");

            if (response.IsSuccessStatusCode)
            {
                return Ok();
            }

            return BadRequest();
        }
    }
}
