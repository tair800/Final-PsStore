using Final.Mvc.ViewModels.GameVMs;
using Final.Mvc.ViewModels.HomeVMs; // Add reference to HomeVM
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

            // Fetch all games
            HttpResponseMessage allGamesResponse = await client.GetAsync("https://localhost:7047/api/Game");
            if (allGamesResponse.IsSuccessStatusCode)
            {
                var allGamesData = await allGamesResponse.Content.ReadAsStringAsync();
                homeVM.Games = JsonConvert.DeserializeObject<List<GameListItemVM>>(allGamesData);
            }
            else
            {
                // Handle error or assign empty list if there's an error
                homeVM.Games = new List<GameListItemVM>();
            }

            // Fetch new games (if this is a different API endpoint, otherwise use the same list)
            HttpResponseMessage newGamesResponse = await client.GetAsync("https://localhost:7047/api/Game/New"); // Assuming this endpoint exists
            if (newGamesResponse.IsSuccessStatusCode)
            {
                var newGamesData = await newGamesResponse.Content.ReadAsStringAsync();
                homeVM.NewGames = JsonConvert.DeserializeObject<List<GameListItemVM>>(newGamesData);
            }
            else
            {
                homeVM.NewGames = new List<GameListItemVM>();
            }

            // Fetch deals (assuming there's a separate API endpoint for deals)
            HttpResponseMessage dealsResponse = await client.GetAsync("https://localhost:7047/api/Game/Deals"); // Assuming this endpoint exists
            if (dealsResponse.IsSuccessStatusCode)
            {
                var dealsData = await dealsResponse.Content.ReadAsStringAsync();
                homeVM.Deals = JsonConvert.DeserializeObject<List<GameListItemVM>>(dealsData);
            }
            else
            {
                homeVM.Deals = new List<GameListItemVM>();
            }

            //// Fetch other necessary data (e.g., categories, promos) if required
            //// Example for categories:
            //HttpResponseMessage categoriesResponse = await client.GetAsync("https://localhost:7047/api/Category"); // Assuming this endpoint exists
            //if (categoriesResponse.IsSuccessStatusCode)
            //{
            //    var categoriesData = await categoriesResponse.Content.ReadAsStringAsync();
            //    homeVM.Categories = JsonConvert.DeserializeObject<List<CategoryVM>>(categoriesData);
            //}
            //else
            //{
            //    homeVM.Categories = new List<CategoryVM>();
            //}

            // Assuming promos are fetched the same way
            HttpResponseMessage promosResponse = await client.GetAsync("https://localhost:7047/api/Promo"); // Assuming this endpoint exists
            if (promosResponse.IsSuccessStatusCode)
            {
                var promosData = await promosResponse.Content.ReadAsStringAsync();
                homeVM.Promos = JsonConvert.DeserializeObject<List<PromoVM>>(promosData);
            }
            else
            {
                homeVM.Promos = new List<PromoVM>();
            }

            // Return the HomeVM to the view
            return View(homeVM);
        }
    }
}
