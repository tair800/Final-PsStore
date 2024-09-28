using Final.Mvc.ViewModels.WishlistVMs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text;

public class WishlistController : Controller
{
    private readonly HttpClient _client;

    public WishlistController(HttpClient client)
    {
        _client = client;
    }

    // Index action to display user's wishlist
    public async Task<IActionResult> Index()
    {
        var token = Request.Cookies["token"];
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "User"); // Redirect to login if no token found
        }

        string userId = GetUserIdFromToken(token); // Get the encoded userId

        // Set the Authorization header with the token
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Make the API call to fetch the wishlist using the encoded userId
        HttpResponseMessage response = await _client.GetAsync($"https://localhost:7047/api/Wishlist/{userId}");

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
        var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            throw new Exception("User ID not found in token");
        }

        var encodedUserId = Convert.ToBase64String(Encoding.UTF8.GetBytes(userId));
        return encodedUserId;
    }
}
