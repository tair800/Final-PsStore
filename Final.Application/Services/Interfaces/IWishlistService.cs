using Final.Core.Entities;

namespace Final.Application.Services.Interfaces
{
    public interface IWishlistService
    {
        // Get the wishlist by userId
        Task<UserWishlistDto> GetWishlistByUser(string userId);

        // Add a game to the wishlist
        Task<Wishlist> Add(string userId, int gameId);

        // Delete a game from the wishlist
        Task<bool> Delete(string userId, int gameId);

        // Clear the entire wishlist for a user (optional if required)
        Task<bool> ClearWishlist(string userId);
    }
}
