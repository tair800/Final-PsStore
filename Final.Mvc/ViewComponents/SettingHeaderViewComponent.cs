﻿using Final.Application.Services.Interfaces;
using Final.Core.Entities;
using Final.Mvc.ViewModels.SettingVMs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Final.Mvc.ViewComponents
{
    [ViewComponent(Name = "SettingHeader")]
    public class SettingHeaderViewComponent : ViewComponent
    {
        private readonly ISettingService _settingService; // Make sure it matches your actual interface
        private readonly IBasketService _basketService;
        private readonly UserManager<User> _userManager;

        public SettingHeaderViewComponent(ISettingService settingService, IBasketService basketService, UserManager<User> userManager)
        {
            _settingService = settingService;
            _basketService = basketService;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            User existUser = null;

            if (User.Identity.IsAuthenticated)
            {
                existUser = await _userManager.FindByEmailAsync(User.Identity.Name);
            }

            int basketCount = 0;
            decimal totalPrice = 0;

            if (existUser != null)
            {
                var basket = await _basketService.GetBasketByEmail(existUser.Email);
                if (basket != null)
                {
                    basketCount = basket.BasketGames.Sum(m => m.Quantity);
                    totalPrice = basket.BasketGames.Sum(m => m.Game.Price * m.Quantity);
                }
            }

            var settings = (await _settingService.GetAll()) // Make sure this method returns a list
                .ToDictionary(s => s.Key, s => s.Value);

            var model = new HeaderVM
            {
                Settings = settings,
                BasketCount = basketCount,
                TotalPrice = totalPrice,
                UserFullName = existUser?.FullName
            };

            return View(model);
        }
    }
}
