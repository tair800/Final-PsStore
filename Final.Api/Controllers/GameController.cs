using Final.Application.Dtos.GameDtos;
using Final.Application.Exceptions;
using Final.Application.Extensions;
using Final.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Final.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;

        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpPost("")]
        public async Task<IActionResult> Create(GameCreateDto gameCreateDto)
        {
            if (gameCreateDto == null)
                return BadRequest();

            return Ok(await _gameService.Create(gameCreateDto));
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAll()
        {
            var games = await _gameService.GetAll();



            //var paginatedGames = games.
            //    Skip((page - 1) * 2)
            //    .Take(2);

            //if (!paginatedGames.Any())
            //{
            //    return NotFound("No games found for the given page.");
            //}

            return Ok(games);
        }


        [HttpGet("Get/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var data = await _gameService.GetOne(id);

            if (data is null)
            {
                throw new CustomExceptions(402, "Name", "Given game name doesnt exist.");
            }

            return Ok(data);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id > 0)
            {
                var game = await _gameService.GetOne(id);
                if (game == null)
                {
                    throw new CustomExceptions(404, "Game", "Game not found.");
                }

                FileExtension.DeleteImage(game.ImgUrl);
                await _gameService.Delete(id);

                return Ok("Game Deleted Successfully.");
            }
            throw new CustomExceptions(400, "Id", "Given id doesnt exist.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] GameUpdateDto gameUpdateDto)
        {
            if (id > 0)
            {
                await _gameService.Update(id, gameUpdateDto);
                return Ok("Game updated successfully.");
            }
            throw new CustomExceptions(404, "Id", "Given id is invalid");
        }

        [HttpGet("Search")]
        public async Task<IActionResult> Search([FromQuery] string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return BadRequest("Search term cannot be empty.");
            }

            var games = await _gameService.SearchGames(title);

            if (games == null || !games.Any())
            {
                return NotFound("No games found.");
            }

            return Ok(games);
        }
    }
}
