using Final.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Final.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketService _basketService;

        public BasketController(IBasketService basketService)
        {
            _basketService = basketService;
        }

        [HttpGet("{email}")]
        public async Task<IActionResult> GetBasket(string email)
        {
            try
            {
                var basket = await _basketService.GetBasketByEmail(email);
                if (basket == null)
                    return NotFound(new { Message = "Basket not found." });

                return Ok(basket);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToBasket(string email, int gameId, int quantity)
        {
            try
            {
                var updatedBasket = await _basketService.Add(email, gameId, quantity);
                return Ok(updatedBasket);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateBasket(string email, int gameId, int quantity)
        {
            try
            {
                var updatedBasket = await _basketService.Update(email, gameId, quantity);
                return Ok(updatedBasket);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveFromBasket(string email, int gameId)
        {
            try
            {
                var result = await _basketService.Delete(email, gameId);
                if (result)
                    return Ok(new { Message = "Game removed from the basket." });
                else
                    return NotFound(new { Message = "Game not found in basket." });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearBasket(string email)
        {
            try
            {
                var result = await _basketService.ClearBasket(email);
                if (result)
                    return Ok(new { Message = "Basket cleared." });
                else
                    return NotFound(new { Message = "Basket not found." });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
