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

            // Deserialize the entire response, including the "data" field
            var apiResponse = JsonConvert.DeserializeObject<dynamic>(data);
            var userWishlistData = apiResponse.data.ToString();  // Extract "data" from the response

            // Deserialize the "data" part to UserWishlistVM
            var userWishListDto = JsonConvert.DeserializeObject<UserWishlistVM>(userWishlistData);

            UserWishlistVM userWishlistVM = new UserWishlistVM()
            {
                UserId = userWishListDto.UserId,
                WishlistGames = userWishListDto.WishlistGames,
            };

            // Return the wishlist view with the populated UserWishlistVM model
            return View(userWishlistVM);
        }

        // If the API call fails, handle appropriately (e.g., show an error message or redirect)
        return RedirectToAction("Error");
    }


    private string GetUserIdFromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        return jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;
    }
}
