using Final.Application.Exceptions;
using Final.Application.Services.Interfaces;
using Final.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Final.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketService _basketService;
        private readonly IHttpContextAccessor _httpContext;
        private readonly UserManager<User> _userManager;

        public BasketController(IBasketService basketService, IHttpContextAccessor httpContext, UserManager<User> userManager)
        {
            _basketService = basketService;
            _httpContext = httpContext;
            _userManager = userManager;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetBasket(string userId)
        {
            try
            {
                var basket = await _basketService.GetBasketByUser(userId);
                if (basket == null)
                    return NotFound(new { Message = "Basket not found." });

                return Ok(basket);
            }
            catch (CustomExceptions ex)
            {
                return StatusCode(ex.Code, new { Message = ex.Message, Errors = ex.Errors });
            }
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToBasket(string userId, int gameId, int quantity)
        {
            try
            {
                await _basketService.Add(userId, gameId, quantity);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateBasket(string userId, int gameId, int quantity)
        {
            try
            {
                var updatedBasket = await _basketService.Update(userId, gameId, quantity);
                return Ok(updatedBasket);
            }
            catch (CustomExceptions ex)
            {
                return StatusCode(ex.Code, new { Message = ex.Message, Errors = ex.Errors });
            }
        }

        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveFromBasket(string userId, int gameId)
        {
            try
            {
                var result = await _basketService.Delete(userId, gameId);
                if (result)
                    return Ok(new { Message = "Game removed from the basket." });
                else
                    return NotFound(new { Message = "Game not found in basket." });
            }
            catch (CustomExceptions ex)
            {
                return StatusCode(ex.Code, new { Message = ex.Message, Errors = ex.Errors });
            }
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearBasket(string userId)
        {
            try
            {
                var result = await _basketService.ClearBasket(userId);
                if (result)
                    return Ok(new { Message = "Basket cleared." });
                else
                    return NotFound(new { Message = "Basket not found." });
            }
            catch (CustomExceptions ex)
            {
                return StatusCode(ex.Code, new { Message = ex.Message, Errors = ex.Errors });
            }
        }
        [HttpPost("add-dlc")]
        public async Task<IActionResult> AddDlcToBasket(string userId, int dlcId, int quantity)
        {
            try
            {
                await _basketService.AddDlc(userId, dlcId, quantity);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // New endpoint for removing DLC from basket
        [HttpDelete("remove-dlc")]
        public async Task<IActionResult> RemoveDlcFromBasket(string userId, int dlcId)
        {
            try
            {
                var result = await _basketService.DeleteDlc(userId, dlcId);
                if (result)
                    return Ok(new { Message = "DLC removed from the basket." });
                else
                    return NotFound(new { Message = "DLC not found in basket." });
            }
            catch (CustomExceptions ex)
            {
                return StatusCode(ex.Code, new { Message = ex.Message, Errors = ex.Errors });
            }
        }

    }
}
