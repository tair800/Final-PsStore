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
        private readonly IEmailService _emailService;

        public GameService(IUnitOfWork unitOfWork, IMapper mapper, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailService = emailService;
        }

        public async Task<int> Create(GameCreateDto createDto)
        {
            if (await _unitOfWork.gameRepository.isExists(g => g.Title.ToLower() == createDto.Title.ToLower()))
                throw new CustomExceptions(400, "Name", "Dublicate is not permitted");

            var game = _mapper.Map<Game>(createDto);
            await _unitOfWork.gameRepository.Create(game);



            _unitOfWork.Commit();


            List<string> emails = new() { "tahir.aslanlee@gmail.com" };
            string body = $"<a href='http://localhost:7047/game/detail/{game.Id}'>Go to Game post</a>";
            _emailService.SendEmail(emails, body, "New Game Post", "View game post");


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
            if (game == null)
            {
                throw new CustomExceptions(404, "Game", "Game not found.");
            }

            var gameReturnDto = _mapper.Map<GameReturnDto>(game);

            // Manually map DLCs if needed
            gameReturnDto.DlcNames = game.Dlcs.Select(dlc => dlc.Name).ToList();

            return gameReturnDto;

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

            if (updateDto.File != null)
            {
                if (!string.IsNullOrEmpty(game.ImgUrl))
                {
                    FileExtension.DeleteImage(game.ImgUrl);
                }

                var newFileName = updateDto.File.Save(Directory.GetCurrentDirectory(), "uploads/images/");
                game.ImgUrl = newFileName;
            }

            game.UpdatedDate = DateTime.Now;

            _mapper.Map(updateDto, game);

            await _unitOfWork.gameRepository.Update(game);
            _unitOfWork.Commit();
        }

        public async Task<List<GameReturnDto>> SearchGames(string title)
        {
            var games = await _unitOfWork.gameRepository.Search(g => g.Title.Contains(title));
            return _mapper.Map<List<GameReturnDto>>(games);
        }


    }
}
