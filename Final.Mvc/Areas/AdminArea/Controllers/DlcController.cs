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

            // Create multipart form data content
            var content = new MultipartFormDataContent();

            // Add name, price, and GameId as string content
            content.Add(new StringContent(model.Name), "Name");
            content.Add(new StringContent(model.Price.ToString()), "Price");
            content.Add(new StringContent(model.GameId.ToString()), "GameId");

            // Add the image if it exists
            if (model.Image != null && model.Image.Length > 0)
            {
                var fileStream = model.Image.OpenReadStream();
                var imageContent = new StreamContent(fileStream);
                imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(model.Image.ContentType);
                content.Add(imageContent, "Image", model.Image.FileName);
            }

            var response = await client.PostAsync("https://localhost:7047/api/Dlc", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Dlc"); // Redirect to DLC list or another page after successful creation
            }

            ModelState.AddModelError("", "There was an error creating the DLC.");
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            // Fetch the DLC details, including the ImgUrl
            var response = await client.GetAsync($"https://localhost:7047/api/Dlc/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            // Deserialize the response into the ViewModel (AdminDlcUpdateVM)
            var data = await response.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<AdminDlcUpdateVM>(data);

            // Fetch the list of games for the dropdown
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

            return View(model); // Pass the fully populated ViewModel to the view
        }


        [HttpPost]
        public async Task<IActionResult> Update(AdminDlcUpdateVM model) //bura beakpoint at go isled
        {
            if (ModelState.IsValid) //File Propertysini null gelir deye Modelstate valid olmur ne? ac mene o file olan hisseni
            {
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

                // Create multipart form data content
                var content = new MultipartFormDataContent();
                content.Add(new StringContent(model.Name), "Name");
                content.Add(new StringContent(model.Price.ToString()), "Price");
                content.Add(new StringContent(model.GameId.ToString()), "GameId");

                if (model.File != null && model.File.Length > 0)
                {
                    // Generate a new filename with GUID
                    var fileName = $"{Guid.NewGuid()}_{model.File.FileName}";

                    // Set the destination folder path
                    var destinationPath = Path.Combine("C:\\Users\\taira\\source\\repos\\Final\\Final.Api\\wwwroot\\uploads\\images", fileName);

                    // Save the file to the destination folder
                    using (var fileStream = new FileStream(destinationPath, FileMode.Create))
                    {
                        await model.File.CopyToAsync(fileStream);
                    }

                    // Create stream content for image upload
                    var imageContent = new StreamContent(model.File.OpenReadStream());
                    imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(model.File.ContentType);
                    content.Add(new StringContent(fileName), "Image");
                }


                var response = await client.PutAsync($"https://localhost:7047/api/Dlc/{model.Id}", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }
            ModelState.AddModelError("", "There was an error updating the DLC.");
            return View(model);
        }

    }
}
