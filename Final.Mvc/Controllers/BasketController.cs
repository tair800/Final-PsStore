using Final.Mvc.ViewModels.BasketVMs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
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
            // Get the user ID (you can fetch it from the claims if needed)
            //var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            //if (string.IsNullOrEmpty(userId))
            //{
            //    return Unauthorized(); // Handle unauthenticated case
            //}

            var token = Request.Cookies["token"];
            if (string.IsNullOrEmpty(token))
            {
                return PartialView("_BasketPartial", new UserBasketVM { BasketGames = new() });
            }

            string userId = null;

            // Decode the JWT token
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Extract claims
            var claims = jwtToken.Claims.ToList();
            userId = claims.FirstOrDefault(c => c.Type == "nameid")?.Value; // Assuming userId is in the "sub" claim

            // Set the Authorization header with the token
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Make the API call to fetch the basket using the userId
            HttpResponseMessage response = await _client.GetAsync($"https://localhost:7047/api/Basket/{userId}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var basketDto = JsonConvert.DeserializeObject<UserBasketVM>(data); // Deserialize into UserBasketVM

                if (basketDto == null || basketDto.BasketGames == null)
                {
                    basketDto = new UserBasketVM { BasketGames = new List<BasketGameVM>() };
                }

                // Return the basket partial view with the populated UserBasketVM model
                return PartialView("_BasketPartial", basketDto);
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error fetching basket: {error}");

                // Return an empty basket partial view in case of failure
                return PartialView("_BasketPartial", new UserBasketVM { BasketGames = new List<BasketGameVM>() });
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddToBasket(int gameId, int quantity)
        {
            var token = Request.Cookies["token"];
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(); // Handle unauthenticated case
            }

            var userId = GetUserIdFromToken(token); // Assume this method extracts userId from the JWT token

            // Call your API to add the game to the basket
            var response = await _client.PostAsync($"https://localhost:7047/api/Basket/add?userId={userId}&gameId={gameId}&quantity={quantity}", null);

            if (response.IsSuccessStatusCode)
            {
                return Ok();
            }

            return BadRequest("Failed to add game to basket.");
        }

        // Helper function to extract userId from the token
        private string GetUserIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;
        }


        [HttpPost]
        public async Task<IActionResult> RemoveFromBasket(int gameId)
        {
            var token = Request.Cookies["token"];
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(); // Handle unauthenticated case
            }

            var userId = GetUserIdFromToken(token); // Extract the userId from the JWT token

            // Call your API to remove the game from the basket
            var response = await _client.DeleteAsync($"https://localhost:7047/api/Basket/remove?userId={userId}&gameId={gameId}");

            if (response.IsSuccessStatusCode)
            {
                return Ok();
            }

            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error removing game from basket: {error}");
            return BadRequest("Failed to remove game from basket.");
        }

    }
}
