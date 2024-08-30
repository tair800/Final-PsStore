using Final.Application.Dtos.GameDtos;
using Final.Core.Entities;

namespace Final.Application.Services.Interfaces
{
    public interface IGameService
    {
        Task<int> Create(GameCreateDto createDto);
        Task<List<Game>> GetAll();
        Task<Game> GetOne(string name);

    }

}
