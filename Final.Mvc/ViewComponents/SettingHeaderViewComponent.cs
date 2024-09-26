using Final.Application.Services.Interfaces;
using Final.Mvc.ViewModels.SettingVMs;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Final.Mvc.ViewComponents
{
    [ViewComponent(Name = "SettingHeader")]
    public class SettingHeaderViewComponent : ViewComponent
    {
        private readonly ISettingService _settingService;
        private readonly IBasketService _basketService;

        public SettingHeaderViewComponent(ISettingService settingService, IBasketService basketService)
        {
            _settingService = settingService;
            _basketService = basketService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Extract the token from the cookie
            var token = Request.Cookies["token"];

            string userName = null;
            string fullName = null;
            string email = null;

            if (!string.IsNullOrEmpty(token))
            {
                // Decode the JWT token
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                // Extract claims
                var claims = jwtToken.Claims.ToList();
                userName = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName)?.Value;
                fullName = claims.FirstOrDefault(c => c.Type == "given_name")?.Value;
                email = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value;
            }

            int basketCount = 0;
            double totalPrice = 0;

            if (!string.IsNullOrEmpty(email))
            {
                var basket = await _basketService.GetBasketByEmail(email);
                if (basket != null)
                {
                    basketCount = basket.BasketGames.Sum(m => m.Quantity);
                    totalPrice = basket.BasketGames.Sum(m => m.Game.Price * m.Quantity);
                }
            }

            var settings = (await _settingService.GetAll())
                .ToDictionary(s => s.Key, s => s.Value);

            var model = new HeaderVM
            {
                Settings = settings,
                BasketCount = basketCount,
                TotalPrice = (int)totalPrice,
                FullName = fullName
            };

            return View(model);
        }
    }
}
