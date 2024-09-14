using Final.Mvc.Areas.AdminArea.ViewModels.DlcVMs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Final.Mvc.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class DlcController : Controller
    {

        private readonly IHttpClientFactory _httpClientFactory;

        public DlcController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            var response = await client.GetAsync("https://localhost:7047/api/Dlc");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var dlcs = JsonConvert.DeserializeObject<List<DlcListVM>>(data);
                return View(dlcs);
            }

            return View(new List<DlcListVM>());
        }

        public async Task<IActionResult> Detail(int id)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            var response = await client.GetAsync($"https://localhost:7047/api/dlc/{id}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var dlcs = JsonConvert.DeserializeObject<AdminDlcReturnVM>(data);
                return View(dlcs);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            var response = await client.DeleteAsync($"https://localhost:7047/api/dlc/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Error deleting the dlc.");
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new AdminDlcCreateVM();

            // Fetch the games from the API to populate the dropdown
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            var gameResponse = await client.GetAsync("https://localhost:7047/api/Game");
            if (gameResponse.IsSuccessStatusCode)
            {
                var gameData = await gameResponse.Content.ReadAsStringAsync();
                model.Games = JsonConvert.DeserializeObject<List<AdminGameVM>>(gameData);
            }
            else
            {
                model.Games = new List<AdminGameVM>();
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AdminDlcCreateVM model)
        {
            if (!ModelState.IsValid)
            {
                var clients = _httpClientFactory.CreateClient();
                var gameResponse = await clients.GetAsync("https://localhost:7047/api/Game");
                if (gameResponse.IsSuccessStatusCode)
                {
                    var gameData = await gameResponse.Content.ReadAsStringAsync();
                    model.Games = JsonConvert.DeserializeObject<List<AdminGameVM>>(gameData);
                }

                return View(model);
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            var jsonContent = JsonConvert.SerializeObject(new
            {
                Name = model.Name,
                Price = model.Price,
                GameId = model.GameId
            });

            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:7047/api/Dlc", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Dlc"); // Redirect to DLC list or another page after successful creation
            }

            ModelState.AddModelError("", "There was an error creating the DLC.");
            return View(model);
        }

    }
}
