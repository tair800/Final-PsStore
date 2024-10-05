using Final.Mvc.ViewModels.PromoVMs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Final.Mvc.ViewComponents
{
    public class PromoViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public PromoViewComponent(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
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

            // Return the view with a list of promos
            return View(promoList);
        }
    }
}
