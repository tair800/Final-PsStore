using Final.Mvc.ViewModels.BasketVMs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

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



    public async Task<IActionResult> Checkout()
    {
        var token = Request.Cookies["token"];
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        string userId = GetUserIdFromToken(token);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Fetch basket for the user
        HttpResponseMessage response = await _client.GetAsync($"https://localhost:7047/api/Basket/{userId}");
        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadAsStringAsync();
            var basketDto = JsonConvert.DeserializeObject<UserBasketVM>(data);
            return View("Checkout", basketDto);
        }
        else
        {
            return RedirectToAction("GetBasket");
        }
    }

    [HttpPost]
    public async Task<IActionResult> PlaceOrder()
    {
        var token = Request.Cookies["token"];
        if (string.IsNullOrEmpty(token))
        {
            return Unauthorized();
        }

        string userId = GetUserIdFromToken(token);
        var response = await _client.PostAsync($"https://localhost:7047/api/Order/place?userId={userId}", null);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("OrderSuccess");
        }

        return BadRequest("Failed to place order.");
    }

    public IActionResult OrderSuccess()
    {
        return View();
    }

    private string GetUserIdFromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        return jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;
    }
}
