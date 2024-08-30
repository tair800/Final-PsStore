using Final.Application.Dtos.GameDtos;
using Final.Core.Entities;

namespace Final.Application.Services.Interfaces
{
    public interface IGameService
    {

        IEnumerable<Game> GetAllGames();

        Task<int> Create(GameCreateDto createDto);
    }

}
