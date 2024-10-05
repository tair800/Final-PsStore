using Final.Mvc.ViewModels.PromoVMs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Final.Mvc.Controllers
{
    public class PromoController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public PromoController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            List<PromoListItemVM> promoList = new List<PromoListItemVM>();

            using (HttpClient client = _httpClientFactory.CreateClient())
            {
                // Set Authorization Header with the Bearer Token from Cookies
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

                HttpResponseMessage response = await client.GetAsync("https://localhost:7047/api/Promo");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    promoList = JsonConvert.DeserializeObject<List<PromoListItemVM>>(data);
                }
                else
                {
                    return BadRequest("Error fetching promo.");
                }
            }

            return View(promoList); // Return promo list to the main view
        }

        [HttpGet]
        public async Task<IActionResult> PromoPartial()
        {
            List<PromoListItemVM> promoList = new List<PromoListItemVM>();

            using (HttpClient client = _httpClientFactory.CreateClient())
            {
                // Set Authorization Header with the Bearer Token from Cookies
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

                HttpResponseMessage response = await client.GetAsync("https://localhost:7047/api/Promo");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    promoList = JsonConvert.DeserializeObject<List<PromoListItemVM>>(data);
                }
            }

            return PartialView("_PromoPartial", promoList); // Return the partial view with promo data
        }
    }
}
