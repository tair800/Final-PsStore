using Final.Mvc.ViewModels.BasketVMs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text;

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
            return PartialView("_BasketPartial", new UserBasketVM { BasketGames = new() });
        }

        string userId = GetUserIdFromToken(token);

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        HttpResponseMessage response = await _client.GetAsync($"https://localhost:7047/api/Basket/{userId}");

        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadAsStringAsync();
            var basketDto = JsonConvert.DeserializeObject<UserBasketVM>(data);

            if (basketDto == null || basketDto.BasketGames == null)
            {
                basketDto = new UserBasketVM { BasketGames = new List<BasketGameVM>() };
            }

            return PartialView("_BasketPartial", basketDto);
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error fetching basket: {error}");
            return PartialView("_BasketPartial", new UserBasketVM { BasketGames = new List<BasketGameVM>() });
        }
    }





    [HttpPost]
    public async Task<IActionResult> AddToBasket(int gameId, int quantity = 1)
    {
        var token = Request.Cookies["token"];
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized();
        }

        string userId = GetUserIdFromToken(token);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Make the API call to add the game to the basket
        HttpResponseMessage response = await _client.PostAsync($"https://localhost:7047/api/Basket/add?userId={userId}&gameId={gameId}&quantity={quantity}", null);

        if (response.IsSuccessStatusCode)
        {
            return Ok("Game added to basket.");
        }
        else
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            return BadRequest($"Failed to add game to basket: {errorMessage}");
        }
    }
    // Method for checkout
    [HttpPost]
    public async Task<IActionResult> Checkout(string selectedCardId, string cardNumber, string expiryMonth, string expiryYear, string cvc)
    {
        var token = Request.Cookies["token"];
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized();
        }

        string userId = GetUserIdFromToken(token);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Fetch basket games
        HttpResponseMessage basketResponse = await _client.GetAsync($"https://localhost:7047/api/Basket/{userId}");
        if (!basketResponse.IsSuccessStatusCode || !basketResponse.Content.ReadAsStringAsync().Result.Contains("BasketGames"))
        {
            return BadRequest("You cannot proceed to checkout with an empty basket.");
        }

        if (string.IsNullOrEmpty(selectedCardId))
        {
            // If no card was selected, save a new card
            var cardData = new
            {
                CardNumber = cardNumber,
                ExpiryMonth = expiryMonth,
                ExpiryYear = expiryYear,
                CVC = cvc
            };

            var content = new StringContent(JsonConvert.SerializeObject(cardData), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"https://localhost:7047/api/User/{userId}/cards", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return BadRequest($"Failed to save card: {error}");
            }
        }

        // Proceed with checkout logic
        HttpResponseMessage checkoutResponse = await _client.PostAsync($"https://localhost:7047/api/Orders/checkout?userId={userId}", null);

        if (!checkoutResponse.IsSuccessStatusCode)
        {
            var error = await checkoutResponse.Content.ReadAsStringAsync();
            return BadRequest($"Checkout failed: {error}");
        }

        // Clear the basket after a successful checkout
        var clearResponse = await _client.DeleteAsync($"https://localhost:7047/api/Basket/clear?userId={userId}");
        if (!clearResponse.IsSuccessStatusCode)
        {
            return BadRequest("Checkout completed, but failed to clear basket.");
        }

        return Ok("Checkout successful.");
    }

    [HttpGet]
    public async Task<IActionResult> CheckoutRedirect()
    {
        var token = Request.Cookies["token"];
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized();
        }

        string userId = GetUserIdFromToken(token);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Check if the user's basket contains any items
        HttpResponseMessage response = await _client.GetAsync($"https://localhost:7047/api/Basket/{userId}");
        if (!response.IsSuccessStatusCode)
        {
            // If the API call fails, handle the error
            TempData["ErrorMessage"] = "Failed to retrieve basket details. Please try again later.";
            return RedirectToAction("GetBasket");
        }

        // Parse the basket response
        var basketData = await response.Content.ReadAsStringAsync();
        var basketVM = JsonConvert.DeserializeObject<UserBasketVM>(basketData);

        // Check if the basket has any items
        if (basketVM == null || basketVM.BasketGames == null || !basketVM.BasketGames.Any())
        {
            TempData["ErrorMessage"] = "Your basket is empty. Add items to proceed.";
            return RedirectToAction("GetBasket");
        }

        // If basket contains items, redirect to the My Cards page
        return RedirectToAction("MyCards", "Payment");
    }


    private string GetUserIdFromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        return jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;
    }
}
