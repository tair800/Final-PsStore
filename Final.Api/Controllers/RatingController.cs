using Final.Application.Dtos.RatingDtos;
using Final.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Final.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly IRatingService _ratingService;

        public RatingController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        // Submit or update a rating
        [HttpPost]
        public async Task<IActionResult> RateGame(RatingCreateDto ratingDto)
        {
            if (ratingDto == null || ratingDto.Score < 1 || ratingDto.Score > 5)
                return BadRequest("Invalid rating. The score must be between 1 and 5.");

            var result = await _ratingService.RateGame(ratingDto);
            return Ok(result);
        }

        // Get the user's rating for a game
        [HttpGet("user/{userId}/game/{gameId}")]
        public async Task<IActionResult> GetUserRating(string userId, int gameId)
        {
            var rating = await _ratingService.GetUserRating(userId, gameId);
            if (rating == null)
                return NotFound("Rating not found.");

            return Ok(rating);
        }

        // Get the average rating for a game
        [HttpGet("game/{gameId}")]
        public async Task<IActionResult> GetAverageRating(int gameId)
        {
            var averageRating = await _ratingService.GetAverageRating(gameId);
            return Ok(new { GameId = gameId, AverageRating = averageRating });
        }
    }

}
