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

        public async Task<IActionResult> Detail(int id)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            var response = await client.GetAsync($"https://localhost:7047/api/Comment/{id}/history");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();

                var apiResult = JsonConvert.DeserializeObject<dynamic>(data);

                var commentDetail = JsonConvert.DeserializeObject<AdminCommentDetailVM>(Convert.ToString(apiResult.comment));

                commentDetail.History = JsonConvert.DeserializeObject<List<CommentHistoryDto>>(Convert.ToString(apiResult.history));

                return View(commentDetail);
            }

            ModelState.AddModelError("", "Error retrieving comment details.");
            return RedirectToAction("Index");
        }


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
