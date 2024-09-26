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
            // Extract the token from cookies
            var token = Request.Cookies["token"];
            if (string.IsNullOrEmpty(token))
            {
                // Log for debugging
                Console.WriteLine("No token found in cookies.");

                // Return the view with a message that the user is not logged in
                return PartialView("_BasketPartial", null);
            }

            using HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await client.GetAsync("https://localhost:7047/api/Basket/{email}"); // Adjust the {email} logic

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var basket = JsonConvert.DeserializeObject<List<BasketItemVM>>(data); // Assuming basket is a list of items

                return PartialView("_BasketPartial", basket); // Returning basket items to the partial view
            }
            else
            {
                // Log for debugging
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error fetching basket: {error}");

                return PartialView("_BasketPartial", null); // Return empty basket if not successful
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
