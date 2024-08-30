using Final.Application.Dtos.GameDtos;
using Final.Application.Exceptions;
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
        public async Task<IActionResult> Get()
        {
            return Ok(await _gameService.GetAll());
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> Get(string name)
        {
            var data = await _gameService.GetOne(name);

            if (data is null)
            {
                throw new CustomExceptions(402, "Name", "Given game name doesnt exist.");
            }

            return Ok(data);
        }
    }
}
