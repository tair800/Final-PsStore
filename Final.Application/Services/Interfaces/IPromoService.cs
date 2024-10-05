using Final.Application.Dtos.PromoDtos;

namespace Final.Application.Services.Interfaces
{
    public interface IPromoService
    {
        Task<int> Create(PromoCreateDto createDto);

        Task<List<PromoReturnDto>> GetAll();

        Task<PromoReturnDto> GetOne(int id);

        Task Delete(int id);

        Task Update(int id, PromoUpdateDto updateDto);
    }
}
