using Final.Application.Dtos.WishlistDtos;
using Final.Application.Exceptions;
using Final.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Final.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        // Get Wishlist by User ID
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetWishlist(string userId)
        {
            var wishlist = await _wishlistService.GetWishlistByUser(userId);
            if (wishlist == null || wishlist.WishlistGames == null || !wishlist.WishlistGames.Any())
            {
                return NotFound(new { Message = "No items found in the wishlist." });
            }

            return Ok(new { Message = "Wishlist retrieved successfully.", Data = wishlist });
        }

        // Add Game to Wishlist
        [HttpPost("add")]
        public async Task<IActionResult> AddToWishlist([FromBody] AddWishlistDto dto)
        {
            if (!ModelState.IsValid)
            {
                // Return structured validation error messages
                return BadRequest(new { Message = "Invalid data.", Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }

            try
            {
                var result = await _wishlistService.Add(dto.UserId, dto.GameId);
                if (result != null)
                {
                    return Ok(new { Message = "Game added to wishlist successfully." });
                }

                return BadRequest(new { Message = "Failed to add game to wishlist." });
            }
            catch (CustomExceptions ex)
            {
                return StatusCode(ex.Code, new { Message = ex.Message, Errors = ex.Errors });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred.", Error = ex.Message });
            }
        }

        // Remove Game from Wishlist
        [HttpDelete("remove/{userId}/{gameId}")]
        public async Task<IActionResult> RemoveFromWishlist(string userId, int gameId)
        {
            try
            {
                var result = await _wishlistService.Delete(userId, gameId);
                if (result)
                {
                    return Ok(new { Message = "Game removed from the wishlist successfully." });
                }

                return NotFound(new { Message = "Game not found in the wishlist." });
            }
            catch (CustomExceptions ex)
            {
                return StatusCode(ex.Code, new { Message = ex.Message, Errors = ex.Errors });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred.", Error = ex.Message });
            }
        }

        // Clear User's Wishlist
        [HttpDelete("clear/{userId}")]
        public async Task<IActionResult> ClearWishlist(string userId)
        {
            try
            {
                var result = await _wishlistService.ClearWishlist(userId);
                if (result)
                {
                    return Ok(new { Message = "Wishlist cleared successfully." });
                }

                return NotFound(new { Message = "Wishlist not found." });
            }
            catch (CustomExceptions ex)
            {
                return StatusCode(ex.Code, new { Message = ex.Message, Errors = ex.Errors });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred.", Error = ex.Message });
            }
        }
    }
}
