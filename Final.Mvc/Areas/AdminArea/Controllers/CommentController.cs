using Final.Mvc.Areas.AdminArea.ViewModels.CommnetVMs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Final.Mvc.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class CommentController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CommentController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // List all comments
        public async Task<IActionResult> Index()

        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            var response = await client.GetAsync("https://localhost:7047/api/Comment");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var comments = JsonConvert.DeserializeObject<List<AdminCommentListVM>>(data);
                return View(comments);
            }

            ModelState.AddModelError("", "Unable to retrieve comments.");
            return View(new List<AdminCommentListVM>());
        }

        // Show the details of a specific comment, including its history
        public async Task<IActionResult> Detail(int id)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            var response = await client.GetAsync($"https://localhost:7047/api/Comment/{id}/history");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();

                // Parse the entire API response into a dynamic object (or anonymous object)
                var apiResult = JsonConvert.DeserializeObject<dynamic>(data);

                // Deserialize the comment portion
                var commentDetail = JsonConvert.DeserializeObject<AdminCommentDetailVM>(Convert.ToString(apiResult.comment));

                // Deserialize the history portion
                commentDetail.History = JsonConvert.DeserializeObject<List<CommentHistoryDto>>(Convert.ToString(apiResult.history));

                return View(commentDetail);
            }

            ModelState.AddModelError("", "Error retrieving comment details.");
            return RedirectToAction("Index");
        }


        // Delete a comment
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            var response = await client.DeleteAsync($"https://localhost:7047/api/Comment/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Comment deleted successfully.";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Error deleting the comment.");
            return RedirectToAction("Index");
        }
    }
}
