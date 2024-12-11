using Final.Application.Dtos.DlcDtos;
using Final.Application.Dtos.GameDtos;
using Final.Application.Dtos.UserDtos;
using Final.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Final.Mvc.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]

    public class DashboardController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public DashboardController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            //Fetch activiry
            var activityResponse = await client.GetAsync("https://localhost:7047/api/Activity/recent");
            if (activityResponse.IsSuccessStatusCode)
            {
                var activityData = await activityResponse.Content.ReadAsStringAsync();
                var recentActivities = JsonConvert.DeserializeObject<List<Activ>>(activityData);
                ViewBag.RecentActivities = recentActivities;
            }

            // Fetch total users
            var userResponse = await client.GetAsync("https://localhost:7047/api/User/Count");
            if (userResponse.IsSuccessStatusCode)
            {
                var userCount = await userResponse.Content.ReadAsStringAsync();
                var userCountResponse = JsonConvert.DeserializeObject<UserCountDto>(userCount);
                ViewBag.TotalUsers = userCountResponse.Count;
            }

            // Fetch total games
            var gameResponse = await client.GetAsync("https://localhost:7047/api/Game/Count");
            if (gameResponse.IsSuccessStatusCode)
            {
                var gameCount = await gameResponse.Content.ReadAsStringAsync();
                var gameCountResponse = JsonConvert.DeserializeObject<GameCountDto>(gameCount);
                ViewBag.TotalGames = gameCountResponse.Count;
            }

            // Fetch total DLCs
            var dlcResponse = await client.GetAsync("https://localhost:7047/api/Dlc/Count");
            if (dlcResponse.IsSuccessStatusCode)
            {
                var dlcCountJson = await dlcResponse.Content.ReadAsStringAsync();
                var dlcCountResponse = JsonConvert.DeserializeObject<DlcCountDto>(dlcCountJson);
                ViewBag.TotalDLCs = dlcCountResponse.Count;
            }



            return View();
        }

        public async Task<IActionResult> Detail(int id)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            var response = await client.GetAsync($"https://localhost:7047/api/Activity/{id}");
            if (response.IsSuccessStatusCode)
            {
                var activityData = await response.Content.ReadAsStringAsync();
                var activity = JsonConvert.DeserializeObject<Activ>(activityData);
                return View(activity);
            }

            return NotFound();
        }



    }
}
