using AutoMapper;
using Final.Application.Dtos.WishlistDtos;
using Final.Application.Exceptions;
using Final.Application.Services.Interfaces;
using Final.Core.Entities;
using Final.Data.Implementations;
using Microsoft.AspNetCore.Identity;

namespace Final.Application.Services.Implementations
{
    public class WishlistService : IWishlistService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public WishlistService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        // Get wishlist by userId
        public async Task<UserWishlistDto> GetWishlistByUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new CustomExceptions(404, "UserNotFound", "User not found.");

            var wishlist = await _unitOfWork.wishlistRepository.GetEntity(w => w.UserId == user.Id, "WishlistGames.Game");
            if (wishlist == null) throw new CustomExceptions(404, "WishlistNotFound", "Wishlist not found.");

            var userWishlistDto = _mapper.Map<UserWishlistDto>(wishlist);
            return userWishlistDto;
        }

        public async Task<Wishlist> Add(string userId, int gameId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new CustomExceptions(404, "UserNotFound", "User not found.");

            var game = await _unitOfWork.gameRepository.GetEntity(g => g.Id == gameId);
            if (game == null) throw new CustomExceptions(404, "GameNotFound", "Game not found.");

            var wishlist = await _unitOfWork.wishlistRepository.GetEntity(w => w.UserId == user.Id, "WishlistGames.Game");

            if (wishlist == null)
            {
                wishlist = new Wishlist
                {
                    UserId = user.Id,
                    WishlistGames = new List<WishlistGame>()
                };

                await _unitOfWork.wishlistRepository.Create(wishlist);
                _unitOfWork.Commit();
            }

            var wishlistGame = wishlist.WishlistGames.FirstOrDefault(wg => wg.GameId == gameId);

            if (wishlistGame != null)
            {
                throw new CustomExceptions(400, "GameAlreadyInWishlist", "Game is already in the wishlist.");
            }
            else
            {
                var newWishlistGame = new WishlistGame
                {
                    WishlistId = wishlist.Id,
                    GameId = gameId
                };

                wishlist.WishlistGames.Add(newWishlistGame);
                await _unitOfWork.wishlistGameRepository.Create(newWishlistGame);
            }

            _unitOfWork.Commit();

            return wishlist;
        }

        // Delete game from wishlist by userId
        public async Task<bool> Delete(string userId, int gameId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new CustomExceptions(404, "UserNotFound", "User not found.");

            var wishlist = await _unitOfWork.wishlistRepository.GetEntity(w => w.UserId == user.Id, "WishlistGames.Game");
            if (wishlist == null) throw new CustomExceptions(404, "WishlistNotFound", "Wishlist not found.");

            var wishlistGame = wishlist.WishlistGames.FirstOrDefault(wg => wg.GameId == gameId);
            if (wishlistGame != null)
            {
                wishlist.WishlistGames.Remove(wishlistGame);
                await _unitOfWork.wishlistGameRepository.Delete(wishlistGame);
                _unitOfWork.Commit();
                return true;
            }

            throw new CustomExceptions(404, "GameNotFound", "Game not found in wishlist.");
        }

        // Clear wishlist by userId
        public async Task<bool> ClearWishlist(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new CustomExceptions(404, "UserNotFound", "User not found.");

            var wishlist = await _unitOfWork.wishlistRepository.GetEntity(w => w.UserId == user.Id, "WishlistGames.Game");
            if (wishlist == null) throw new CustomExceptions(404, "WishlistNotFound", "Wishlist not found.");

            foreach (var wishlistGame in wishlist.WishlistGames.ToList())
            {
                await _unitOfWork.wishlistGameRepository.Delete(wishlistGame);
            }

            _unitOfWork.Commit();
            return true;
        }
    }
}
