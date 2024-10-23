using Final.Application.Dtos.RatingDtos;

namespace Final.Application.Services.Interfaces
{
    public interface IRatingService
    {
        Task<double> GetAverageRating(int gameId);
        Task<RatingReturnDto> GetUserRating(string userId, int gameId);
        Task<RatingReturnDto> RateGame(RatingCreateDto ratingDto);
    }
}
