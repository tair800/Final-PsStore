using Final.Application.Services.Interfaces;
using Final.Mvc.ViewModels.SettingVMs;
using Microsoft.AspNetCore.Mvc;

namespace Final.Mvc.ViewComponents
{
    [ViewComponent(Name = "SettingFooter")]
    public class SettingFooterViewComponent : ViewComponent
    {
        private readonly ISettingService _settingService;

        public SettingFooterViewComponent(ISettingService settingService)
        {
            _settingService = settingService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var settings = (await _settingService.GetAll())
                .ToDictionary(s => s.Key, s => s.Value);

            var model = new FooterVM
            {
                Settings = settings
            };

            return View(model);
        }
    }
}
