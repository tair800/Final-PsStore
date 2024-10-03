using Final.Core.Entities;
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
        public async Task<IActionResult> Index(int? category = null, int? platform = null, string sortByPrice = null, string sortByDate = null)
        {
            using HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            HttpResponseMessage response = await client.GetAsync("https://localhost:7047/api/Game");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<List<GameListItemVM>>(data);

                var distinctCategories = result
                    .Select(g => new { g.CategoryId, g.CategoryName })
                    .Distinct()
                    .ToList();

                if (category.HasValue)
                {
                    result = result.Where(g => g.CategoryId == category).ToList();
                }

                if (platform.HasValue)
                {
                    result = result.Where(g => (int)g.Platform == platform).ToList();
                }

                switch (sortByPrice)
                {
                    case "price_asc":
                        result = result.OrderBy(g => g.Price).ToList();
                        break;
                    case "price_desc":
                        result = result.OrderByDescending(g => g.Price).ToList();
                        break;
                    default:
                        break;
                }

                // Sort by release date
                switch (sortByDate)
                {
                    case "date_asc":
                        result = result.OrderBy(g => g.CreatedDate).ToList();
                        break;
                    case "date_desc":
                        result = result.OrderByDescending(g => g.CreatedDate).ToList();
                        break;
                    default:
                        break;
                }

                ViewBag.Categories = distinctCategories;

                return View(result);
            }

            return BadRequest("Error fetching games.");
        }

        public async Task<IActionResult> Detail(int id)
        {
            using HttpClient client = new();
            var token = Request.Cookies["token"];

            // Check if the user is authenticated
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                ViewBag.IsAuthenticated = true;
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
        public async Task<IActionResult> AddComment(GameDetailWithCommentsVM gameDetailWithCommentsVM)
        {
            var token = Request.Cookies["token"];

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "User");
            }

            var userId = GetUserIdFromToken(token);

            var newComment = new Comment
            {
                Content = gameDetailWithCommentsVM.ContentNew.Content,
                GameId = gameDetailWithCommentsVM.ContentNew.GameId,
                UserId = userId, // Assuming you're setting the user ID here
                CreatedDate = DateTime.Now
            };

            if (string.IsNullOrEmpty(newComment.Content) || newComment.GameId == 0 || string.IsNullOrEmpty(newComment.UserId))
            {
                TempData["Error"] = "Invalid comment data.";
                return RedirectToAction("Detail", new { id = newComment.GameId });
            }

            using HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var jsonContent = JsonConvert.SerializeObject(newComment);
            StringContent content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("https://localhost:7047/api/Comment", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Detail", new { id = newComment.GameId });
            }

            TempData["Error"] = "Failed to add comment.";
            return RedirectToAction("Detail", new { id = newComment.GameId });
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
