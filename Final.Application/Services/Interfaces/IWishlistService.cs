using Final.Application.Dtos.WishlistDtos;
using Final.Core.Entities;

namespace Final.Application.Services.Interfaces
{
    public interface IWishlistService
    {
        Task<UserWishlistDto> GetWishlistByUser(string userId);

        Task<Wishlist> Add(string userId, int gameId);

        Task<bool> Delete(string userId, int gameId);

        Task<bool> ClearWishlist(string userId);
    }
}
