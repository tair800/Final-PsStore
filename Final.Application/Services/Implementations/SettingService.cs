using AutoMapper;
using Final.Application.Dtos.SettingsDto;
using Final.Application.Services.Interfaces;
using Final.Core.Entities;
using Final.Data.Implementations;

namespace Final.Application.Services.Implementations
{
    public class SettingService : ISettingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SettingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<SettingsReturnDto> Get(string key)
        {
            var setting = await _unitOfWork.settingsRepository.GetEntity(s => s.Key == key);
            return setting == null ? null : _mapper.Map<SettingsReturnDto>(setting);
        }

        public async Task<List<SettingsReturnDto>> GetAll()
        {
            var settings = await _unitOfWork.settingsRepository.GetAll();
            return _mapper.Map<List<SettingsReturnDto>>(settings);
        }

        public async Task Create(SettingCreateDto settingsDto)
        {
            var existingSetting = await _unitOfWork.settingsRepository.GetEntity(s => s.Key == settingsDto.Key);
            if (existingSetting != null)
            {
                throw new InvalidOperationException($"A setting with the key '{settingsDto.Key}' already exists.");
            }

            var newSetting = _mapper.Map<Setting>(settingsDto);
            await _unitOfWork.settingsRepository.Create(newSetting);
            _unitOfWork.Commit();
        }

        public async Task Delete(int id)
        {
            var setting = await _unitOfWork.settingsRepository.GetEntity(s => s.Id == id);
            if (setting != null)
            {
                await _unitOfWork.settingsRepository.Delete(setting);
                _unitOfWork.Commit();
            }
            else
            {
                throw new KeyNotFoundException("Setting not found.");
            }
        }

        public async Task Update(string key, SettingUpdateDto settingsUpdateDto)
        {
            var existingSetting = await _unitOfWork.settingsRepository.GetEntity(s => s.Key == key);
            if (existingSetting == null)
            {
                throw new KeyNotFoundException($"No setting found with the key '{key}'.");
            }

            if (!string.IsNullOrEmpty(settingsUpdateDto.Key) && settingsUpdateDto.Key != key)
            {
                var duplicateSetting = await _unitOfWork.settingsRepository.GetEntity(s => s.Key == settingsUpdateDto.Key);
                if (duplicateSetting != null)
                {
                    throw new InvalidOperationException($"A setting with the key '{settingsUpdateDto.Key}' already exists.");
                }

                existingSetting.Key = settingsUpdateDto.Key;
            }

            if (!string.IsNullOrEmpty(settingsUpdateDto.Value))
            {
                existingSetting.Value = settingsUpdateDto.Value;
            }

            await _unitOfWork.settingsRepository.Update(existingSetting);
            _unitOfWork.Commit();
        }



    }
}
