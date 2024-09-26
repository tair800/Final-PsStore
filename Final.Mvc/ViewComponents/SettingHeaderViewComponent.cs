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

            string userId = null;
            string fullName = null;

            if (!string.IsNullOrEmpty(token))
            {
                // Decode the JWT token
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                // Extract claims
                var claims = jwtToken.Claims.ToList();
                userId = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value; // Assuming userId is in the "sub" claim
                fullName = claims.FirstOrDefault(c => c.Type == "given_name")?.Value;
            }

            int basketCount = 0;
            decimal totalPrice = 0;

            if (!string.IsNullOrEmpty(userId))
            {
                var basket = await _basketService.GetBasketByUser(userId);
                if (basket != null)
                {
                    basketCount = basket.BasketGames.Sum(m => m.Quantity);
                    totalPrice = basket.BasketGames.Sum(m => m.Price * m.Quantity);  // Ensure the price is properly fetched from BasketGame or Game
                }
            }

            var settings = (await _settingService.GetAll())
                .ToDictionary(s => s.Key, s => s.Value);

            var model = new HeaderVM
            {
                Settings = settings,
                BasketCount = basketCount,
                TotalPrice = totalPrice,  // No need to cast to int, use decimal or double
                FullName = fullName
            };

            return View(model);
        }
    }
}
