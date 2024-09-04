using Final.Application.Dtos.GameDtos;

namespace Final.Application.Services.Interfaces
{
    public interface IGameService
    {
        Task<int> Create(GameCreateDto createDto);
        Task<List<GameReturnDto>> GetAll();
        Task<GameReturnDto> GetOne(string title);
        Task Delete(string title);
        Task Update(string title, GameUpdateDto gameUpdateDto);
    }

}
