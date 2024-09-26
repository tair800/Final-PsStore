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

        // GET: api/Wishlist/{userId}
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetWishlist(string userId)
        {
            try
            {
                var wishlist = await _wishlistService.GetWishlistByUser(userId);
                if (wishlist == null)
                {
                    return NotFound(new { Message = "Wishlist not found." });
                }

                return Ok(wishlist);
            }
            catch (CustomExceptions ex)
            {
                return StatusCode(ex.Code, new { Message = ex.Message, Errors = ex.Errors });
            }
        }

        // POST: api/Wishlist/add
        //[Authorize]
        [HttpPost("add")]
        public async Task<IActionResult> AddToWishlist([FromBody] AddWishlistDto request)
        {
            try
            {
                var wishlist = await _wishlistService.Add(request.UserId, request.GameId);
                return Ok(wishlist);
            }
            catch (CustomExceptions ex)
            {
                return StatusCode(ex.Code, new { Message = ex.Message, Errors = ex.Errors });
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
                    return Ok(new { Message = "Game removed from the wishlist." });
                }
                return NotFound(new { Message = "Game not found in wishlist." });
            }
            catch (CustomExceptions ex)
            {
                return StatusCode(ex.Code, new { Message = ex.Message, Errors = ex.Errors });
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
                    return Ok(new { Message = "Wishlist cleared." });
                }
                return NotFound(new { Message = "Wishlist not found." });
            }
            catch (CustomExceptions ex)
            {
                return StatusCode(ex.Code, new { Message = ex.Message, Errors = ex.Errors });
            }
        }
    }
}
