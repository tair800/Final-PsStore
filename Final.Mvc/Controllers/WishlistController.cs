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
            return RedirectToAction("Login", "User");
        }

        // Extract UserId from the token
        var userId = GetUserIdFromToken(token);
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("Login", "User");
        }

        using HttpClient client = new();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        HttpResponseMessage response = await client.GetAsync($"https://localhost:7047/api/wishlist/{userId}");

        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadAsStringAsync();

            var apiResponse = JsonConvert.DeserializeObject<dynamic>(data);
            var userWishlistData = apiResponse.data.ToString();

            var userWishListDto = JsonConvert.DeserializeObject<UserWishlistVM>(userWishlistData);

            // Check if the wishlist is empty and return an empty view model if so
            if (userWishListDto == null || userWishListDto.WishlistGames == null)
            {
                return View(new UserWishlistVM
                {
                    UserId = userId,
                    WishlistGames = new List<WishlistGameVM>()
                });
            }

            UserWishlistVM userWishlistVM = new UserWishlistVM()
            {
                UserId = userWishListDto.UserId,
                WishlistGames = userWishListDto.WishlistGames,
            };

            return View(userWishlistVM);
        }

        // Handle other non-success status codes by showing an empty view
        return View(new UserWishlistVM
        {
            UserId = userId,
            WishlistGames = new List<WishlistGameVM>()
        });
    }
    public async Task<IActionResult> RemoveFromWishlist(string gameId)
    {
        var token = Request.Cookies["token"];
        if (string.IsNullOrEmpty(token))
        {
            return Json(new { success = false, message = "Please log in to remove games from the wishlist." });
        }

        // Extract UserId from the token
        var userId = GetUserIdFromToken(token);
        if (string.IsNullOrEmpty(userId))
        {
            return Json(new { success = false, message = "Invalid user." });
        }

        using HttpClient client = new();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Send the request to the API with parameters in the URL
        HttpResponseMessage response = await client.DeleteAsync($"https://localhost:7047/api/wishlist/remove/{userId}/{gameId}");

        if (response.IsSuccessStatusCode)
        {
            return Json(new { success = true, message = "Game removed from wishlist successfully." });
        }

        var errorResponse = await response.Content.ReadAsStringAsync();
        return Json(new { success = false, message = errorResponse });
    }

    private string GetUserIdFromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        return jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;
    }
}
