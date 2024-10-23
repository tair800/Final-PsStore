using Final.Mvc.Areas.AdminArea.ViewModels.PromoVMs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Final.Mvc.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class PromoController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public PromoController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // List all Promos
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            var response = await client.GetAsync("https://localhost:7047/api/Promo");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var promos = JsonConvert.DeserializeObject<List<AdminPromoListVM>>(data);
                return View(promos);
            }

            return View(new List<AdminPromoListVM>());
        }

        public async Task<IActionResult> Detail(int id)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            var response = await client.GetAsync($"https://localhost:7047/api/promo/{id}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var promo = JsonConvert.DeserializeObject<AdminPromoDetailVM>(data);
                return View(promo);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            var response = await client.DeleteAsync($"https://localhost:7047/api/promo/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Error deleting the promo.");
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new AdminPromoCreateVM();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AdminPromoCreateVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            // Create multipart form data content
            var content = new MultipartFormDataContent();

            // Add name and file as string content
            content.Add(new StringContent(model.Name), "Name");

            // Add the image if it exists
            if (model.File != null && model.File.Length > 0)
            {
                var fileStream = model.File.OpenReadStream();
                var imageContent = new StreamContent(fileStream);
                imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(model.File.ContentType);
                content.Add(imageContent, "File", model.File.FileName);
            }

            var response = await client.PostAsync("https://localhost:7047/api/promo/create", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Promo"); // Redirect to Promo list or another page after successful creation
            }

            ModelState.AddModelError("", "There was an error creating the promo.");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            var response = await client.GetAsync($"https://localhost:7047/api/promo/{id}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var promo = JsonConvert.DeserializeObject<AdminPromoUpdateVM>(data);
                return View(promo);
            }

            return RedirectToAction("Index");
        }

        // Handle Update Promo Form Submission
        [HttpPost]
        public async Task<IActionResult> Update(AdminPromoUpdateVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            // Create multipart form data content
            var content = new MultipartFormDataContent();
            content.Add(new StringContent(model.Name), "Name");

            // Add the image if it exists
            if (model.File != null && model.File.Length > 0)
            {
                var fileStream = model.File.OpenReadStream();
                var imageContent = new StreamContent(fileStream);
                imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(model.File.ContentType);
                content.Add(imageContent, "File", model.File.FileName);
            }

            var response = await client.PutAsync($"https://localhost:7047/api/promo/{model.Id}", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Promo");
            }

            ModelState.AddModelError("", "There was an error updating the promo.");
            return View(model);
        }
    }
}
