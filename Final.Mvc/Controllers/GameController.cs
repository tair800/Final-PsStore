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

                // Set CanDelete property for each comment
                foreach (var comment in comments)
                {
                    comment.CanDelete = comment.UserId == userId;
                    comment.CreateDate = DateTime.UtcNow;
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
        public async Task<IActionResult> AddComment(GameDetailWithCommentsVM gameDetailWithCommentsVM)
        {
            var token = Request.Cookies["token"];

            if (string.IsNullOrEmpty(token))
            {
                return Json(new { success = false, message = "User is not authenticated." });
            }

            var userId = GetUserIdFromToken(token);

            var newComment = new Comment
            {
                Content = gameDetailWithCommentsVM.ContentNew.Content,
                GameId = gameDetailWithCommentsVM.ContentNew.GameId,
                UserId = userId,
                CreatedDate = DateTime.UtcNow,
            };

            if (string.IsNullOrEmpty(newComment.Content) || newComment.GameId == 0 || string.IsNullOrEmpty(newComment.UserId))
            {
                return Json(new { success = false, message = "Invalid comment data." });
            }

            using HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var jsonContent = JsonConvert.SerializeObject(newComment);
            StringContent content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("https://localhost:7047/api/Comment", content);

            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Comment added successfully.", comment = newComment });
            }

            return Json(new { success = false, message = "Failed to add comment." });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var token = Request.Cookies["token"];

            if (string.IsNullOrEmpty(token))
            {
                return Json(new { success = false, message = "User is not authenticated." });
            }

            using HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await client.DeleteAsync($"https://localhost:7047/api/Comment/{commentId}");

            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Comment deleted successfully." });
            }

            return Json(new { success = false, message = "Failed to delete the comment." });
        }

        [HttpPost]
        public async Task<IActionResult> EditComment(int commentId, string content)
        {
            var token = Request.Cookies["token"];

            if (string.IsNullOrEmpty(token))
            {
                return Json(new { success = false, message = "User is not authenticated." });
            }

            using HttpClient client = new();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var editedComment = new
            {
                Content = content,
                CommentId = commentId,
                Modified = true // Mark this comment as modified
            };

            var jsonContent = JsonConvert.SerializeObject(editedComment);
            StringContent contentToSend = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PutAsync($"https://localhost:7047/api/Comment/{commentId}", contentToSend);

            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Comment edited successfully.", newContent = content, modified = true });
            }

            return Json(new { success = false, message = "Failed to edit the comment." });
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
