using AutoMapper;
using Final.Application.Dtos.BasketDtos;
using Final.Application.Exceptions;
using Final.Application.Services.Interfaces;
using Final.Core.Entities;
using Final.Data.Implementations;
using Microsoft.AspNetCore.Identity;

namespace Final.Application.Services.Implementations
{
    public class BasketService : IBasketService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public BasketService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        // Get basket by userId
        public async Task<UserBasketDto> GetBasketByUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new CustomExceptions(404, "UserNotFound", "User not found.");

            var basket = await _unitOfWork.basketRepository.GetEntity(b => b.UserId == user.Id, "BasketGames.Game");
            if (basket == null) throw new CustomExceptions(404, "BasketNotFound", "Basket not found.");

            var userBasketDto = _mapper.Map<UserBasketDto>(basket);
            return userBasketDto;
        }

        // Add game to basket by userId
        public async Task<Basket> Add(string userId, int gameId, int quantity)
        {
            // Check if user exists
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new CustomExceptions(404, "UserNotFound", "User not found.");

            // Check if game exists
            var game = await _unitOfWork.gameRepository.GetEntity(g => g.Id == gameId);
            if (game == null) throw new CustomExceptions(404, "GameNotFound", "Game not found.");

            // Check if basket exists for the user
            var basket = await _unitOfWork.basketRepository.GetEntity(b => b.UserId == user.Id, "BasketGames.Game");

            // If basket does not exist, create a new one
            if (basket == null)
            {
                basket = new Basket
                {
                    UserId = user.Id,
                    BasketGames = new List<BasketGame>()
                };

                // Create a new basket in the database
                await _unitOfWork.basketRepository.Create(basket);
                _unitOfWork.Commit();  // Ensure the basket is saved before adding games
            }

            // Check if the game already exists in the user's basket
            var basketGame = basket.BasketGames.FirstOrDefault(bg => bg.GameId == gameId);

            // If the game is already in the basket, update the quantity
            if (basketGame != null)
            {
                basketGame.Quantity += quantity;
                await _unitOfWork.basketGameRepository.Update(basketGame);
            }
            else
            {
                // Add the game to the basket if it's not already there
                var newBasketGame = new BasketGame
                {
                    BasketId = basket.Id,
                    GameId = gameId,
                    Quantity = quantity
                };

                // Add the new game to the basket
                basket.BasketGames.Add(newBasketGame);
                await _unitOfWork.basketGameRepository.Create(newBasketGame);
            }

            // Commit the transaction to the database
            _unitOfWork.Commit();

            // Return the updated basket
            return basket;
        }


        // Update game quantity in basket by userId
        public async Task<Basket> Update(string userId, int gameId, int quantity)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new CustomExceptions(404, "UserNotFound", "User not found.");

            var basket = await _unitOfWork.basketRepository.GetEntity(b => b.UserId == user.Id, "BasketGames.Game");
            if (basket == null) throw new CustomExceptions(404, "BasketNotFound", "Basket not found.");

            var basketGame = basket.BasketGames.FirstOrDefault(bg => bg.GameId == gameId);

            if (basketGame != null)
            {
                basketGame.Quantity = quantity;
                await _unitOfWork.basketGameRepository.Update(basketGame);
                _unitOfWork.Commit();
            }
            else
            {
                throw new CustomExceptions(404, "GameNotFound", "Game not found in basket.");
            }

            return basket;
        }

        // Delete game from basket by userId
        public async Task<bool> Delete(string userId, int gameId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new CustomExceptions(404, "UserNotFound", "User not found.");

            var basket = await _unitOfWork.basketRepository.GetEntity(b => b.UserId == user.Id, "BasketGames.Game");
            if (basket == null) throw new CustomExceptions(404, "BasketNotFound", "Basket not found.");

            var basketGame = basket.BasketGames.FirstOrDefault(bg => bg.GameId == gameId);
            if (basketGame != null)
            {
                basket.BasketGames.Remove(basketGame);
                await _unitOfWork.basketGameRepository.Delete(basketGame);
                _unitOfWork.Commit();
                return true;
            }

            throw new CustomExceptions(404, "GameNotFound", "Game not found in basket.");
        }

        // Clear basket by userId
        public async Task<bool> ClearBasket(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new CustomExceptions(404, "UserNotFound", "User not found.");

            var basket = await _unitOfWork.basketRepository.GetEntity(b => b.UserId == user.Id, "BasketGames.Game");
            if (basket == null) throw new CustomExceptions(404, "BasketNotFound", "Basket not found.");

            foreach (var basketGame in basket.BasketGames.ToList())
            {
                await _unitOfWork.basketGameRepository.Delete(basketGame);
            }

            _unitOfWork.Commit();
            return true;
        }





        //public async Task<Basketdr> UpdateGameQuantity(string userId, string gameId, int change)
        //{
        //    var userBasket = await _unitOfWork.Baskets.GetBasketByUserIdAsync(userId);
        //    if (userBasket == null)
        //    {
        //        throw new CustomExceptions(404, "Basket not found for the given user.");
        //    }

        //    var basketGame = userBasket.BasketGames.FirstOrDefault(g => g.GameId == gameId);
        //    if (basketGame == null)
        //    {
        //        throw new CustomExceptions(404, "Game not found in the basket.");
        //    }

        //    // Update the quantity of the game
        //    basketGame.Quantity += change;

        //    // If quantity is less than or equal to 0, remove the game from the basket
        //    if (basketGame.Quantity <= 0)
        //    {
        //        userBasket.BasketGames.Remove(basketGame);
        //    }

        //    // Save changes to the database
        //    await _unitOfWork.CompleteAsync();

        //    // Use AutoMapper to map the updated basket entity to a BasketDto
        //    return _mapper.Map<BasketDto>(userBasket);
        ////}
    }
}
