using Final.Mvc.ViewModels.CommentVMs;
using Final.Mvc.ViewModels.GameVMs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

namespace Final.Mvc.Controllers
{
    public class GameController : Controller
    {

        private readonly IHttpClientFactory _httpClientFactory;

        public GameController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index(int? category = null, int? platform = null, string sortByPrice = null, string sortByDate = null)
        {
            using HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            // Fetch all games
            HttpResponseMessage gameResponse = await client.GetAsync("https://localhost:7047/api/Game");

            if (!gameResponse.IsSuccessStatusCode)
            {
                return BadRequest("Error fetching games.");
            }

            var gameData = await gameResponse.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<GameListItemVM>>(gameData);

            // Extract userId from the token
            var token = Request.Cookies["token"];
            string userId = GetUserIdFromToken(token);

            // Fetch the user's wishlist
            HttpResponseMessage wishlistResponse = await client.GetAsync($"https://localhost:7047/api/Game/UserWishlist?userId={userId}");

            if (!wishlistResponse.IsSuccessStatusCode)
            {
                return BadRequest("Error fetching wishlist.");
            }

            var wishlistData = await wishlistResponse.Content.ReadAsStringAsync();
            var wishlistGames = JsonConvert.DeserializeObject<List<GameListItemVM>>(wishlistData);

            // Mark games that are in the user's wishlist
            var wishlistGameIds = wishlistGames.Select(g => g.Id).ToList();
            foreach (var game in result)
            {
                game.IsInWishlist = wishlistGameIds.Contains(game.Id);
            }

            // Filtering logic (category, platform, etc.)
            if (category.HasValue)
            {
                result = result.Where(g => g.CategoryId == category).ToList();
            }

            if (platform.HasValue)
            {
                result = result.Where(g => (int)g.Platform == platform).ToList();
            }

            // Sorting logic (price, date, etc.)
            switch (sortByPrice)
            {
                case "price_asc":
                    result = result.OrderBy(g => g.Price).ToList();
                    break;
                case "price_desc":
                    result = result.OrderByDescending(g => g.Price).ToList();
                    break;
            }

            switch (sortByDate)
            {
                case "date_asc":
                    result = result.OrderBy(g => g.CreatedDate).ToList();
                    break;
                case "date_desc":
                    result = result.OrderByDescending(g => g.CreatedDate).ToList();
                    break;
            }

            ViewBag.Categories = result
                .Select(g => new { g.CategoryId, g.CategoryName })
                .Distinct()
                .ToList();

            return View(result);
        }

        public async Task<IActionResult> Detail(int id)
        {
            using HttpClient client = new();
            var token = Request.Cookies["token"];

            string userId = null;

            // Check if the user is authenticated
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                ViewBag.IsAuthenticated = true;

                // Extract user ID from the token
                userId = GetUserIdFromToken(token);
            }
            else
            {
                ViewBag.IsAuthenticated = false;
            }

            // Fetch game details
            HttpResponseMessage gameResponse = await client.GetAsync($"https://localhost:7047/api/Game/Get/{id}");
            if (!gameResponse.IsSuccessStatusCode)
            {
                return NotFound("Game not found.");
            }

            var gameData = await gameResponse.Content.ReadAsStringAsync();
            var gameResult = JsonConvert.DeserializeObject<GameDetailVM>(gameData);

            // Fetch comments related to the game
            HttpResponseMessage commentResponse = await client.GetAsync($"https://localhost:7047/api/Comment/game/{id}");
            List<CommentListItemVM> comments = new List<CommentListItemVM>();

            if (commentResponse.IsSuccessStatusCode)
            {
                var commentData = await commentResponse.Content.ReadAsStringAsync();
                comments = JsonConvert.DeserializeObject<List<CommentListItemVM>>(commentData);

                // Set CanDelete property for each comment and use correct dates
                foreach (var comment in comments)
                {
                    comment.CanDelete = comment.UserId == userId;
                    if (comment.UpdatedDate.HasValue)
                    {
                        comment.UpdatedDate = TimeZoneInfo.ConvertTimeFromUtc(comment.UpdatedDate.Value, TimeZoneInfo.FindSystemTimeZoneById("Azerbaijan Standard Time")); // Or any other time zone that represents UTC+4
                    }
                }
            }

            var viewModel = new GameDetailWithCommentsVM
            {
                GameDetail = gameResult,
                Contents = comments,
                ContentNew = new CommentCreateVM()
            };

            return View(viewModel);
        }





        [HttpPost]
        public async Task<IActionResult> AddComment([FromBody] CommentCreateVM model)
        {
            using var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            // Extract UserId from JWT token (if token contains this claim)
            var token = Request.Cookies["token"];
            if (token != null)
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;

                // Ensure UserId is set in the model
                model.UserId = userId;
            }

            var content = new StringContent(JsonConvert.SerializeObject(model), System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:7047/api/Comment/addComment", content);

            if (response.IsSuccessStatusCode)
            {
                return Ok();
            }

            // Log response content for further debugging
            var errorMessage = await response.Content.ReadAsStringAsync();
            return BadRequest($"Failed to add comment: {errorMessage}");
        }


        // Update a comment
        [HttpPut]
        public async Task<IActionResult> UpdateComment(int id, [FromBody] CommentUpdateVM model)
        {
            using var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            var content = new StringContent(JsonConvert.SerializeObject(model), System.Text.Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"https://localhost:7047/api/Comment/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                return Ok();
            }
            return BadRequest("Failed to update comment.");
        }

        // Delete a comment
        [HttpDelete]
        public async Task<IActionResult> DeleteComment(int id)
        {
            using var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            var response = await client.DeleteAsync($"https://localhost:7047/api/Comment/{id}");

            if (response.IsSuccessStatusCode)
            {
                return Ok();
            }
            return BadRequest("Failed to delete comment.");
        }



        [HttpPost]
        public async Task<IActionResult> AddToWishlist(string gameId)
        {
            using var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            // Extract UserId from JWT token
            var token = Request.Cookies["token"];
            var userId = GetUserIdFromToken(token);

            var dto = new { UserId = userId, GameId = gameId };
            var content = new StringContent(JsonConvert.SerializeObject(dto), System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://localhost:7047/api/Wishlist/add", content);

            if (response.IsSuccessStatusCode)
            {
                return Ok("Game added to wishlist.");
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            return BadRequest($"Failed to add game to wishlist: {errorMessage}");
        }


        [HttpDelete]
        public async Task<IActionResult> RemoveFromWishlist(string gameId)
        {
            using var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            // Extract UserId from JWT token
            var token = Request.Cookies["token"];
            var userId = GetUserIdFromToken(token);

            var response = await client.DeleteAsync($"https://localhost:7047/api/Wishlist/remove/{userId}/{gameId}");

            if (response.IsSuccessStatusCode)
            {
                return Ok("Game removed from wishlist.");
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            return BadRequest($"Failed to remove game from wishlist: {errorMessage}");
        }



        private string GetUserIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;
        }

        [HttpGet]
        public async Task<IActionResult> Search(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return PartialView("_GameSearch", new List<GameListItemVM>());
            }

            using HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            HttpResponseMessage response = await client.GetAsync($"https://localhost:7047/api/Game/Search?title={title}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var searchResults = JsonConvert.DeserializeObject<List<GameListItemVM>>(data);

                return PartialView("_GameSearch", searchResults);
            }

            return PartialView("_GameSearch", new List<GameListItemVM>()); // Empty list on failure
        }


    }
}
