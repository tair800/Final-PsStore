using AutoMapper;
using Final.Application.Dtos.GameDtos;
using Final.Application.Exceptions;
using Final.Application.Extensions;
using Final.Application.Services.Interfaces;
using Final.Core.Entities;
using Final.Data.Implementations;
using Newtonsoft.Json;

namespace Final.Application.Services.Implementations
{
    public class GameService : IGameService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IActivityService _activityService;

        public GameService(IUnitOfWork unitOfWork, IMapper mapper, IActivityService activityService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _activityService = activityService;
        }

        public async Task<int> Create(GameCreateDto createDto)
        {
            if (await _unitOfWork.gameRepository.isExists(g => g.Title.ToLower() == createDto.Title.ToLower()))
                throw new CustomExceptions(400, "Name", "Duplicate is not permitted");

            var game = _mapper.Map<Game>(createDto);
            await _unitOfWork.gameRepository.Create(game);
            _unitOfWork.Commit();

            await _activityService.LogActivity(new Activ
            {
                ActionType = "Create",
                EntityName = "Game",
                Details = $"Game '{game.Title}' was created.",
                PerformedBy = "System",
                EntityId = game.Id
            });

            return game.Id;
        }

        public async Task<List<GameReturnDto>> GetAll()
        {
            var games = await _unitOfWork.gameRepository.GetAll(null, "Dlcs", "Category");
            var gameDtos = _mapper.Map<List<GameReturnDto>>(games);

            foreach (var gameDto in gameDtos)
            {
                var ratings = await _unitOfWork.ratingRepository.GetAll(r => r.GameId == gameDto.Id);
                gameDto.AverageRating = ratings.Any() ? ratings.Average(r => r.Score) : 0;
            }

            return gameDtos;
        }

        public async Task<List<GameReturnDto>> GetAllUserWishlist(string userId)
        {
            var wishlist = await _unitOfWork.wishlistRepository.GetEntity(w => w.UserId == userId, "WishlistGames.Game");

            if (wishlist == null || !wishlist.WishlistGames.Any())
            {
                return new List<GameReturnDto>();
            }

            var games = wishlist.WishlistGames.Select(wg => wg.Game).ToList();

            return _mapper.Map<List<GameReturnDto>>(games);
        }



        public async Task<GameReturnDto> GetOne(int id)
        {
            var game = await _unitOfWork.gameRepository.GetEntity(g => g.Id == id, "Dlcs", "Category", "Ratings");

            if (game == null)
            {
                throw new CustomExceptions(404, "Game", "Game not found.");
            }

            var averageRating = game.Ratings.Any() ? game.Ratings.Average(r => r.Score) : 0;
            var ratingCount = game.Ratings.Count();

            var gameReturnDto = _mapper.Map<GameReturnDto>(game);
            gameReturnDto.AverageRating = averageRating;
            gameReturnDto.RatingCount = ratingCount;

            return gameReturnDto;
        }


        public async Task Delete(int id)
        {
            var game = await _unitOfWork.gameRepository.GetEntity(g => g.Id == id);
            if (game is null)
                throw new CustomExceptions(404, "Game", "Game not found.");

            await _unitOfWork.gameRepository.Delete(game);
            _unitOfWork.Commit();


            await _activityService.LogActivity(new Activ
            {
                ActionType = "Delete",
                EntityName = "Game",
                Details = $"Game '{game.Title}' with ID {id} was deleted.",
                PerformedBy = "System"
            });


        }


        public async Task Update(int id, GameUpdateDto updateDto)
        {
            var game = await _unitOfWork.gameRepository.GetEntity(g => g.Id == id);
            if (game == null)
                throw new CustomExceptions(404, "Game", "Game not found.");

            var jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                DateFormatString = "yyyy-MM-dd HH:mm:ss"
            };

            var oldData = JsonConvert.SerializeObject(game, jsonSettings);
            var oldDataDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(oldData);
            var propertiesToExclude = new HashSet<string> { "WishlistGames", "Comments", "BasketGames", "Dlcs", "Ratings" };

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

            var newData = JsonConvert.SerializeObject(game, jsonSettings);
            var newDataDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(newData);

            var changedProperties = new Dictionary<string, (object oldValue, object newValue)>();
            foreach (var key in oldDataDict.Keys)
            {
                if (propertiesToExclude.Contains(key))
                    continue;

                if (newDataDict.ContainsKey(key) && !Equals(oldDataDict[key], newDataDict[key]))
                {
                    changedProperties[key] = (oldDataDict[key], newDataDict[key]);
                }
            }

            var formattedOldData = JsonConvert.SerializeObject(
                changedProperties.ToDictionary(k => k.Key, v => v.Value.oldValue),
                jsonSettings
            );

            var formattedNewData = JsonConvert.SerializeObject(
                changedProperties.ToDictionary(k => k.Key, v => v.Value.newValue),
                jsonSettings
            );

            await _unitOfWork.gameRepository.Update(game);
            _unitOfWork.Commit();

            await _activityService.LogActivity(new Activ
            {
                ActionType = "Update",
                EntityName = "Game",
                Details = $"Game '{game.Title}' with ID {id} was updated.",
                OldData = formattedOldData,
                NewData = formattedNewData,
                PerformedBy = "System",
                EntityId = game.Id
            });
        }



        // Search games by title
        public async Task<List<GameReturnDto>> SearchGames(string title)
        {
            var games = await _unitOfWork.gameRepository.Search(g => g.Title.Contains(title));
            return _mapper.Map<List<GameReturnDto>>(games);
        }
        public async Task<int> Count()
        {
            var count = (await _unitOfWork.gameRepository.GetAll()).Count();
            return count;
        }
    }
}