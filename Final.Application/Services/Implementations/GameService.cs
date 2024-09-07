using AutoMapper;
using Final.Application.Dtos.GameDtos;
using Final.Application.Exceptions;
using Final.Application.Extensions;
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

        public async Task<GameReturnDto> GetOne(int id)
        {
            var game = await _unitOfWork.gameRepository.GetEntity(g => g.Id == id, "Dlcs", "Category");
            return _mapper.Map<GameReturnDto>(game);
        }

        public async Task Delete(int id)
        {
            var game = await _unitOfWork.gameRepository.GetEntity(g => g.Id == id);
            if (game is null)
                throw new CustomExceptions(404, "Game", "Game not found.");

            await _unitOfWork.gameRepository.Delete(game);
            _unitOfWork.Commit();
        }

        public async Task Update(int id, GameUpdateDto updateDto)
        {
            var game = await _unitOfWork.gameRepository.GetEntity(g => g.Id == id);

            if (game == null)
                throw new CustomExceptions(404, "Game", "Game not found.");

            FileExtension.DeleteImage(game.ImgUrl);
            var filName = updateDto.File.Save(Directory.GetCurrentDirectory(), "uploads/images/");

            var existGame = _mapper.Map(updateDto, game);
            existGame.ImgUrl = filName;

            await _unitOfWork.gameRepository.Update(existGame);
            _unitOfWork.Commit();
        }
    }
}
