using Final.Mvc.ViewModels.CategoryVMs;
using Final.Mvc.ViewModels.GameVMs;
using Final.Mvc.ViewModels.HomeVMs;
using Final.Mvc.ViewModels.PromoVMs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Final.Mvc.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            // Create a new instance of HomeVM to store the fetched data
            var homeVM = new HomeVM();

            // Fetch all games in one API call
            HttpResponseMessage allGamesResponse = await client.GetAsync("https://localhost:7047/api/Game");
            if (allGamesResponse.IsSuccessStatusCode)
            {
                var allGamesData = await allGamesResponse.Content.ReadAsStringAsync();
                var allGames = JsonConvert.DeserializeObject<List<GameListItemVM>>(allGamesData);

                homeVM.Games = allGames;

                homeVM.NewGames = allGames
                    .Where(g => g.CreatedDate >= DateTime.Now.AddDays(-5))
                    .ToList();

                homeVM.Deals = allGames
                    .Where(g => g.SalePrice.HasValue)
                    .ToList();
            }
            else
            {
                homeVM.Games = new List<GameListItemVM>();
                homeVM.NewGames = new List<GameListItemVM>();
                homeVM.Deals = new List<GameListItemVM>();
            }

            // Fetch promos
            HttpResponseMessage promosResponse = await client.GetAsync("https://localhost:7047/api/Promo");
            if (promosResponse.IsSuccessStatusCode)
            {
                var promosData = await promosResponse.Content.ReadAsStringAsync();
                homeVM.Promos = JsonConvert.DeserializeObject<List<PromoVM>>(promosData);
            }
            else
            {
                homeVM.Promos = new List<PromoVM>();
            }

            // Fetch categories
            HttpResponseMessage categoriesResponse = await client.GetAsync("https://localhost:7047/api/Category");
            if (categoriesResponse.IsSuccessStatusCode)
            {
                var categoriesData = await categoriesResponse.Content.ReadAsStringAsync();
                homeVM.Categories = JsonConvert.DeserializeObject<List<CategoryFirstVM>>(categoriesData);
            }
            else
            {
                homeVM.Categories = new List<CategoryFirstVM>();
            }

            // Return the HomeVM to the view
            return View(homeVM);
        }
    }
}
