using AutoMapper;
using Final.Application.Dtos.GameDtos;
using Final.Application.Exceptions;
using Final.Application.Services.Interfaces;
using Final.Core.Entities;
using Final.Data.Implementations;

namespace Final.Application.Services.Implementations
{
    public class GameService : IGameService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GameService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<int> Create(GameCreateDto createDto)
        {
            if (await _unitOfWork.gameRepository.isExists(g => g.Title.ToLower() == createDto.Title.ToLower()))
                throw new CustomExceptions(400, "Name", "Dublicate is not permitted");

            var game = _mapper.Map<Game>(createDto);
            await _unitOfWork.gameRepository.Create(game);

            foreach (var platformId in createDto.PlatformIds)
            {
                var gamePlatform = new GamePlatform
                {
                    GameId = game.Id,
                    PlatformId = platformId
                };
                await _unitOfWork.gamePlatformRepository.Create(gamePlatform);
            }

            _unitOfWork.Commit();

            return game.Id;
        }

        public async Task<List<Game>> GetAll() => await _unitOfWork.gameRepository.GetAll();

        public async Task<Game> GetOne(string name)
        {
            return await _unitOfWork.gameRepository.GetEntity(g => g.Title == name);
        }
    }
}
