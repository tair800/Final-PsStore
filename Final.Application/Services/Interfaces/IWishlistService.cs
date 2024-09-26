using Final.Application.Dtos.WisihlistDtos;
using Final.Core.Entities;

namespace Final.Application.Services.Interfaces
{
    public interface IWishlistService
    {
        Task<Wishlist> Get(string userId);
        Task<bool> Add(string userId, WishlistDto item);
        Task<bool> Delete(string userId, int itemId);
    }
}
