using Final.Mvc.ViewModels.WishlistVMs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Final.Mvc.Controllers
{
    public class WishlistController : Controller
    {
        private readonly HttpClient _client;

        public WishlistController(HttpClient client)
        {
            _client = client;
        }

        // Display the wishlist items in an Index view
        public async Task<IActionResult> Index()
        {
            var token = Request.Cookies["token"];
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("No token found in cookies.");
                return RedirectToAction("Login", "User");
            }

            // Set the Authorization header with the JWT token
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Get the userId from the JWT token
            var userId = Request.Cookies["userId"];  // Make sure you're storing userId in the cookie
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("No userId found in cookies.");
                return RedirectToAction("Login", "User");
            }

            using HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await client.GetAsync("https://localhost:7047/api/Wishlist/get/{userId}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var wishlist = JsonConvert.DeserializeObject<List<WishlistItemVM>>(data); // Ensure this deserialization step is correct
                return View("Index", wishlist); // Ensure you're passing the correct data to the view
            }
            else
            {
                // Log for debugging
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error fetching wishlist: {error}");

                return View("Index", new List<WishlistItemVM>()); // Return an empty wishlist if there's an error
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddToWishlist(int gameId)
        {
            // Check if the user is logged in
            var userId = Request.Cookies["userId"];
            var token = Request.Cookies["token"];

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                // If the user is not logged in, redirect to the login page
                return RedirectToAction("Login", "User");
            }

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Call the API to add the game to the wishlist
            var response = await _client.PostAsync($"https://localhost:7047/api/Wishlist/add?userId={userId}&gameId={gameId}", null);

            if (response.IsSuccessStatusCode)
            {
                return Ok();
            }

            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromWishlist(int gameId)
        {
            // Check if the user is logged in
            var userId = Request.Cookies["userId"];
            var token = Request.Cookies["token"];

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                // If the user is not logged in, redirect to the login page
                return RedirectToAction("Login", "User");
            }

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Call the API to remove the game from the wishlist
            var response = await _client.DeleteAsync($"https://localhost:7047/api/Wishlist/remove?userId={userId}&gameId={gameId}");

            if (response.IsSuccessStatusCode)
            {
                return Ok();
            }

            return BadRequest();
        }
    }
}
