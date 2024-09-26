using Final.Application.Dtos.WisihlistDtos;
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
        //[Authorize]
        public async Task<IActionResult> Get(string userId)
        {
            var wishlist = await _wishlistService.Get(userId);
            if (wishlist == null)
                return NotFound("Wishlist not found.");


            return Ok(wishlist);
        }

        [HttpPost("{userId}/add")]
        //[Authorize]
        public async Task<IActionResult> Add(string userId, [FromBody] WishlistDto wishlistItem)
        {
            var result = await _wishlistService.Add(userId, wishlistItem);
            if (!result)
                return BadRequest("Failed to add item to wishlist.");


            return Ok("Item added to wishlist.");
        }

        [HttpDelete("{userId}/delete/{gameId}")]
        //[Authorize]
        public async Task<IActionResult> Remove(string userId, int gameId)
        {
            var result = await _wishlistService.Delete(userId, gameId);
            if (!result)
                return BadRequest("Failed to remove item from wishlist.");


            return Ok("Item removed from wishlist.");
        }
    }
}
