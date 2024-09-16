using Final.Application.Dtos.SettingsDto;
using Final.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Final.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingController : ControllerBase
    {
        private readonly ISettingService _settingService;

        public SettingController(ISettingService settingService)
        {
            _settingService = settingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSettings()
        {
            var settings = await _settingService.GetAll();
            return Ok(settings);
        }

        [HttpGet("{key}")]
        public async Task<IActionResult> GetSettingByKey(string key)
        {
            var setting = await _settingService.Get(key);
            if (setting == null)
            {
                return NotFound(new { Message = $"Setting with key '{key}' not found." });
            }

            return Ok(setting);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSetting([FromBody] SettingCreateDto settingsCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _settingService.Create(settingsCreateDto);
                return StatusCode(201, new { Message = "Setting created successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Message = ex.Message });
            }
        }

        [HttpPut("{key}")]
        public async Task<IActionResult> Update(string key, [FromBody] SettingUpdateDto settingsUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Find the setting by the key from the URL
                var existingSetting = await _settingService.Get(key);
                if (existingSetting == null)
                {
                    return NotFound(new { Message = $"Setting with key '{key}' not found." });
                }

                // Update key and/or value
                await _settingService.Update(key, settingsUpdateDto);

                return Ok(new { Message = "Setting updated successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSetting(int id)
        {
            try
            {
                await _settingService.Delete(id);
                return Ok(new { Message = "Setting deleted successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
    }
}
