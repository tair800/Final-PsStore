using Final.Application.Dtos.SettingsDto;

namespace Final.Application.Services.Interfaces
{
    public interface ISettingService
    {
        Task<SettingsReturnDto> Get(string key);
        Task<List<SettingsReturnDto>> GetAll();
        Task Create(SettingCreateDto settingsDto);
        Task Update(string key, SettingUpdateDto updateDto);
        Task Delete(int id);
    }
}
