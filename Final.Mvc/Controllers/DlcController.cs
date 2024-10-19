using Final.Mvc.ViewModels.DlcVMs;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Final.Mvc.Controllers
{
    public class DlcController : Controller
    {
        private readonly HttpClient _httpClient;

        public DlcController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Index action to list all DLCs
        public async Task<IActionResult> Index()
        {
            // Call your API endpoint to get all DLCs
            var response = await _httpClient.GetAsync("https://localhost:7047/api/dlc");
            if (!response.IsSuccessStatusCode)
            {
                // Handle error
                return View(new List<DlcListVM>());
            }

            var content = await response.Content.ReadAsStringAsync();
            var dlcs = JsonSerializer.Deserialize<IEnumerable<DlcListVM>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return View(dlcs);
        }

        // Detail action to show details of a specific DLC
        public async Task<IActionResult> Detail(int id)
        {
            // Call your API endpoint to get details of a specific DLC
            var response = await _httpClient.GetAsync($"https://localhost:7047/api/dlc/{id}");
            if (!response.IsSuccessStatusCode)
            {
                // Handle error
                return NotFound();
            }

            var content = await response.Content.ReadAsStringAsync();
            var dlc = JsonSerializer.Deserialize<DlcDetailVM>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return View(dlc);
        }
    }
}
