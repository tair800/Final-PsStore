using Final.Application.Dtos.GameDtos;

namespace Final.Application.Services.Interfaces
{
    public interface IGameService
    {
        Task<int> Create(GameCreateDto createDto);
        Task<List<GameReturnDto>> GetAll();
        Task<GameReturnDto> GetOne(int id);
        Task Delete(int id);
        Task Update(int id, GameUpdateDto updateDto);
        Task<List<GameReturnDto>> SearchGames(string title);
        Task<List<GameReturnDto>> GetAllUserWishlist(string userId);
        Task<int> Count();
    }

}
