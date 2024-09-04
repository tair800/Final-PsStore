using Final.Application.Dtos.DlcDtos;

namespace Final.Application.Services.Interfaces
{
    public interface IDlcService
    {
        Task<int> Create(DlcCreateDto dlcCreateDto);
        Task<List<DlcReturnDto>> GetAll();
        Task Delete(string name);
        Task<DlcReturnDto> GetOne(string name);
    }
}
