using AutoMapper;
using Final.Application.Dtos.RatingDtos;
using Final.Application.Services.Interfaces;
using Final.Core.Entities;
using Final.Data.Implementations;

namespace Final.Application.Services.Implementations
{
    public class RatingService : IRatingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RatingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // Submit or update a rating
        public async Task<RatingReturnDto> RateGame(RatingCreateDto ratingDto)
        {
            var rating = await _unitOfWork.ratingRepository.GetEntity(r => r.GameId == ratingDto.GameId && r.UserId == ratingDto.UserId);
            if (rating != null)
            {
                // Update existing rating
                rating.Score = ratingDto.Score;
                await _unitOfWork.ratingRepository.Update(rating);
            }
            else
            {
                // Add new rating
                rating = _mapper.Map<Rating>(ratingDto);
                await _unitOfWork.ratingRepository.Create(rating);
            }
            _unitOfWork.Commit();

            return _mapper.Map<RatingReturnDto>(rating);
        }

        // Get a user's rating for a game
        public async Task<RatingReturnDto> GetUserRating(string userId, int gameId)
        {
            var rating = await _unitOfWork.ratingRepository.GetEntity(r => r.GameId == gameId && r.UserId == userId);
            return _mapper.Map<RatingReturnDto>(rating);
        }

        // Get the average rating for a game
        public async Task<double> GetAverageRating(int gameId)
        {
            var ratings = await _unitOfWork.ratingRepository.GetAll(r => r.GameId == gameId);
            if (!ratings.Any())
                return 0;

            return ratings.Average(r => r.Score);
        }
    }

}
