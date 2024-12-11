using Final.Application.Services.Interfaces;
using Final.Application.ViewModels;
using Final.Mvc.Areas.AdminArea.ViewModels.GameVMs;
using Final.Mvc.Areas.AdminArea.ViewModels.UserVMs;
using Final.Mvc.ViewModels.UserVMs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;

namespace Final.Mvc.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]

    public class GameController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IEmailService emailService;

        public GameController(IHttpClientFactory httpClientFactory, IEmailService emailService)
        {
            _httpClientFactory = httpClientFactory;
            this.emailService = emailService;
        }

        public async Task<IActionResult> Index(string searchTerm = null, int pageNumber = 1, int pageSize = 10)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            var response = await client.GetAsync("https://localhost:7047/api/Game/ForAdmin");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var games = JsonConvert.DeserializeObject<List<GameListVM>>(data);

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    games = games.Where(g => g.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                // Implement pagination logic
                var totalGames = games.Count();
                var paginatedGames = games
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                ViewBag.CurrentPage = pageNumber;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalPages = (int)Math.Ceiling(totalGames / (double)pageSize);
                ViewBag.SearchTerm = searchTerm;

                return View(paginatedGames);
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Login", "User", new { area = "" });
            }
            {

            }

            return View(new List<GameListVM>());
        }




        public async Task<IActionResult> Detail(int id)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            var response = await client.GetAsync($"https://localhost:7047/api/Game/Get/{id}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var game = JsonConvert.DeserializeObject<AdminGameReturnVM>(data);
                return View(game);
            }

            return RedirectToAction("Index");
        }




        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            var response = await client.DeleteAsync($"https://localhost:7047/api/Game/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Error deleting the game.");
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            var response = await client.GetAsync($"https://localhost:7047/api/Game/Get/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            var data = await response.Content.ReadAsStringAsync();
            var game = JsonConvert.DeserializeObject<AdminGameUpdateVM>(data);

            var categoryResponse = await client.GetAsync("https://localhost:7047/api/Category");
            if (categoryResponse.IsSuccessStatusCode)
            {
                var categoryData = await categoryResponse.Content.ReadAsStringAsync();
                game.Categories = JsonConvert.DeserializeObject<List<CategoryVM>>(categoryData);
            }

            return View(game);
        }

        [HttpPost]
        public async Task<IActionResult> Update(AdminGameUpdateVM model)
        {
            if (!ModelState.IsValid)
            {
                // Reload categories if validation fails
                var clients = _httpClientFactory.CreateClient();
                var categoryResponse = await clients.GetAsync("https://localhost:7047/api/Category");
                if (categoryResponse.IsSuccessStatusCode)
                {
                    var categoryData = await categoryResponse.Content.ReadAsStringAsync();
                    model.Categories = JsonConvert.DeserializeObject<List<CategoryVM>>(categoryData);
                }

                return View(model);
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            var formContent = new MultipartFormDataContent
    {
        { new StringContent(model.Title ?? ""), "Title" },
        { new StringContent(model.Description ?? ""), "Description" },
        { new StringContent(model.Price?.ToString() ?? "0"), "Price" },
        { new StringContent(model.SalePrice?.ToString() ?? "0"), "SalePrice" },
        { new StringContent(model.CategoryId.ToString() ?? "0"), "CategoryId" },
        { new StringContent(model.CategoryName  ?? ""), "CategoryName" },
        { new StringContent(model.Platform?.ToString() ?? ""), "Platform" }
    };

            if (model.File != null)
            {
                var fileContent = new StreamContent(model.File.OpenReadStream());
                formContent.Add(fileContent, "File", model.File.FileName);
            }

            var response = await client.PutAsync($"https://localhost:7047/api/Game/{model.Id}", formContent);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Error updating the game.");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new AdminGameCreateVM();

            // Fetch categories
            var client = _httpClientFactory.CreateClient();
            var categoryResponse = await client.GetAsync("https://localhost:7047/api/Category");

            if (categoryResponse.IsSuccessStatusCode)
            {
                var categoryData = await categoryResponse.Content.ReadAsStringAsync();
                model.Categories = JsonConvert.DeserializeObject<List<AdminCategoryVM>>(categoryData);
            }
            else
            {
                model.Categories = new List<AdminCategoryVM>();
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AdminGameCreateVM model)
        {
            if (!ModelState.IsValid)
            {
                var clients = _httpClientFactory.CreateClient();
                var categoryResponse = await clients.GetAsync("https://localhost:7047/api/Category");
                if (categoryResponse.IsSuccessStatusCode)
                {
                    var categoryData = await categoryResponse.Content.ReadAsStringAsync();
                    model.Categories = JsonConvert.DeserializeObject<List<AdminCategoryVM>>(categoryData);
                }
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            var formContent = new MultipartFormDataContent
{
    { new StringContent(model.Title ?? ""), "Title" },
    { new StringContent(model.Description ?? ""), "Description" },
    { new StringContent(model.Price.ToString()), "Price" },
    { new StringContent(model.SalePrice?.ToString() ?? "0"), "SalePrice" },
    { new StringContent(model.CategoryId.ToString()), "CategoryId" },
    { new StringContent(model.Platform.ToString()), "Platform" }
};

            if (model.ImgUrl != null)
            {
                var fileContent = new StreamContent(model.ImgUrl.OpenReadStream());
                formContent.Add(fileContent, "ImgUrl", model.ImgUrl.FileName);
            }

            // Send request to create the game
            var response = await client.PostAsync("https://localhost:7047/api/Game", formContent);

            if (!response.IsSuccessStatusCode)
            {
                // Handle error response
                var errorContent = await response.Content.ReadAsStringAsync();

                try
                {
                    var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(errorContent);

                    if (errorResponse?.Errors != null)
                    {
                        foreach (var error in errorResponse.Errors)
                        {
                            foreach (var errorMessage in error.Value)
                            {
                                ModelState.AddModelError(error.Key, errorMessage);
                            }
                        }
                    }
                    else
                    {
                        ViewBag.ErrorMessage = errorResponse?.Message ?? "An unexpected error occurred.";
                    }
                }
                catch (JsonException ex)
                {
                    // Log the error and show a generic message
                    Console.WriteLine($"Error parsing JSON: {ex.Message}");
                    ViewBag.ErrorMessage = $"Error creating game: {response.StatusCode}. Response: {errorContent}";
                }

                // Fetch categories again to repopulate dropdown after API failure
                var categoryResponse = await client.GetAsync("https://localhost:7047/api/Category");
                if (categoryResponse.IsSuccessStatusCode)
                {
                    var categoryData = await categoryResponse.Content.ReadAsStringAsync();
                    model.Categories = JsonConvert.DeserializeObject<List<AdminCategoryVM>>(categoryData);
                }

                return View(model); // Return the view with the error messages
            }


            // Fetch the list of verified users
            HttpResponseMessage userResponse = await client.GetAsync("https://localhost:7047/api/user/verified");
            if (userResponse.IsSuccessStatusCode)
            {
                var userContent = await userResponse.Content.ReadAsStringAsync();
                var verifiedUsers = JsonConvert.DeserializeObject<List<VerifiedUserVM>>(userContent);

                foreach (var user in verifiedUsers)
                {
                    string body;
                    using (StreamReader sr = new StreamReader("wwwroot/templates/gameTemplate/newGameNotification.html"))
                    {
                        body = sr.ReadToEnd();
                    }

                    body = body
                        .Replace("{{UserName}}", user.Email)
                        .Replace("{{GameTitle}}", model.Title)
                        .Replace("{{GameDescription}}", model.Description);

                    // Send email to the verified user
                    emailService.SendEmail(
                        from: "tahiraa@code.edu.az",
                        to: user.Email,
                        subject: "New Game Created: " + model.Title,
                        body: body,
                        smtpHost: "smtp.gmail.com",
                        smtpPort: 587,
                        enableSsl: true,
                        smtpUser: "tahiraa@code.edu.az",
                        smtpPass: "cark zrzn cjid cjlr"
                    );
                }
            }

            // Redirect to Index after successful creation and email notifications
            TempData["SuccessMessage"] = "Game created successfully and notifications sent to verified users.";
            return RedirectToAction("Index");
        }








    }
}