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



            _unitOfWork.Commit();

            return game.Id;
        }

        public async Task<List<GameReturnDto>> GetAll()
        {
            var games = await _unitOfWork.gameRepository.GetAll(null, "Dlcs", "Category");
            return _mapper.Map<List<GameReturnDto>>(games);
        }

        public async Task<GameReturnDto> GetOne(string title)
        {
            var game = await _unitOfWork.gameRepository.GetEntity(g => g.Title == title, "Dlcs", "Category");
            return _mapper.Map<GameReturnDto>(game);
        }

        public async Task Delete(string title)
        {
            var game = await _unitOfWork.gameRepository.GetEntity(g => g.Title.ToLower() == title.ToLower());
            if (game is null)
                throw new CustomExceptions(404, "Game", "Game not found.");

            await _unitOfWork.gameRepository.Delete(game);
            _unitOfWork.Commit();
        }

        public async Task Update(string title, GameUpdateDto gameUpdateDto)
        {
            var game = await _unitOfWork.gameRepository.GetEntity(g => g.Title.ToLower() == title.ToLower());
            if (game is null)
                throw new CustomExceptions(404, "Game", "Game not found.");

            _mapper.Map(gameUpdateDto, game);

            await _unitOfWork.gameRepository.Update(game);

        }
    }
}
