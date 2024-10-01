using Final.Mvc.ViewModels.WishlistVMs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

public class WishlistController : Controller
{


    // Index action to display user's wishlist
    public async Task<IActionResult> Index()
    {
        var token = Request.Cookies["token"];
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login");
        }

        // Extract UserId from the token
        var userId = GetUserIdFromToken(token);
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("Login");
        }

        using HttpClient client = new();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Make the API call to fetch the wishlist using the userId
        HttpResponseMessage response = await client.GetAsync($"https://localhost:7047/api/wishlist/{userId}");

        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadAsStringAsync();
            var wishlistDto = JsonConvert.DeserializeObject<UserWishlistVM>(data); // Deserialize into UserWishlistVM

            if (wishlistDto == null || wishlistDto.WishlistGames == null)
            {
                wishlistDto = new UserWishlistVM { WishlistGames = new List<WishlistGameVM>() };
            }

            // Return the wishlist view with the populated UserWishlistVM model
            return View(wishlistDto);
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error fetching wishlist: {error}");

            // Return an empty wishlist view in case of failure
            return View(new UserWishlistVM { WishlistGames = new List<WishlistGameVM>() });
        }
    }

    private string GetUserIdFromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        return jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;
    }
}
