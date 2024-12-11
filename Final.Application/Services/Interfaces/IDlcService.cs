using Final.Application.Dtos.DlcDtos;

namespace Final.Application.Services.Interfaces
{
    public interface IDlcService
    {
        Task<int> Create(DlcCreateDto dlcCreateDto);
        Task<List<DlcReturnDto>> GetAll();
        Task Delete(int id);
        Task<DlcReturnDto> GetOne(int id);
        Task Update(int id, DlcUpdateDto dlcUpdateDto);
        Task<int> GetCount();
    }
}
