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

        // POST: api/Wishlist/add
        //[Authorize]
        [HttpPost("add")]
        public async Task<IActionResult> AddToWishlist([FromBody] AddWishlistDto request)
        {
            try
            {
                var wishlist = await _wishlistService.Add(request.UserId, request.GameId);
                if (wishlist == null)
                {
                    return BadRequest(new { Message = "Failed to add the game to the wishlist." });
                }

                return Ok(new { Message = "Game added to wishlist successfully.", Data = wishlist });
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

        // DELETE: api/Wishlist/remove
        //[Authorize]
        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveFromWishlist([FromBody] RemoveWishlistDto request)
        {
            try
            {
                var result = await _wishlistService.Delete(request.UserId, request.GameId);
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

        // DELETE: api/Wishlist/clear/{userId}
        //[Authorize]
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
