using AutoMapper;
using Final.Application.Dtos.WisihlistDtos;
using Final.Application.Services.Interfaces;
using Final.Core.Entities;
using Final.Data.Implementations;

public class WishlistService : IWishlistService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public WishlistService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Wishlist> Get(string userId)
    {
        // Fetch wishlist by userId
        // Fix the Include path to use WishlistGames instead of WishlistItems
        var wishlist = await _unitOfWork.wishlistRepository.GetEntity(w => w.UserId == userId, "WishlistGames.Game");
        if (wishlist == null) throw new Exception("Wishlist not found.");
        return wishlist;
    }

    public async Task<bool> Add(string userId, WishlistDto item)
    {
        var user = await _unitOfWork.userRepository.GetEntity(u => u.Id == userId);
        if (user == null) throw new Exception("User not found.");

        var wishlist = await _unitOfWork.wishlistRepository.GetEntity(w => w.UserId == userId, "WishlistGames.Game");

        if (wishlist == null)
        {
            wishlist = new Wishlist { UserId = user.Id, WishlistGames = new List<WishlistGame>() };
            await _unitOfWork.wishlistRepository.Create(wishlist);
            _unitOfWork.Commit();
        }

        var wishlistItem = wishlist.WishlistGames.FirstOrDefault(wi => wi.GameId == item.Id);

        if (wishlistItem == null)
        {
            // Add new game to the wishlist
            var newWishlistGame = new WishlistGame
            {
                WishlistId = wishlist.Id,
                GameId = item.Id
            };
            await _unitOfWork.wishlistGameRepository.Create(newWishlistGame);
            _unitOfWork.Commit();
            return true;
        }
        else
        {
            throw new Exception("Game is already in the wishlist.");
        }
    }

    public async Task<bool> Delete(string userId, int itemId)
    {
        var wishlist = await _unitOfWork.wishlistRepository.GetEntity(w => w.UserId == userId, "WishlistGames.Game");
        if (wishlist == null) throw new Exception("Wishlist not found.");

        var wishlistItem = wishlist.WishlistGames.FirstOrDefault(wi => wi.GameId == itemId);
        if (wishlistItem != null)
        {
            wishlist.WishlistGames.Remove(wishlistItem);
            await _unitOfWork.wishlistGameRepository.Delete(wishlistItem);
            _unitOfWork.Commit();
            return true;
        }

        return false;
    }
}
