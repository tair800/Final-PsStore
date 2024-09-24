using AutoMapper;
using Final.Application.Dtos.BasketDtos;
using Final.Application.Services.Interfaces;
using Final.Core.Entities;
using Final.Data.Implementations;

namespace Final.Application.Services.Implementations
{
    public class BasketService : IBasketService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BasketService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<UserBasketDto> GetBasketByEmail(string email)
        {
            var user = await _unitOfWork.userRepository.GetEntity(u => u.Email == email, "Basket.BasketGames.Game");
            if (user == null) throw new Exception("User not found.");

            var basket = await _unitOfWork.basketRepository.GetEntity(b => b.UserId == user.Id, "BasketGames.Game");
            if (basket == null) throw new Exception("Basket not found.");

            var userBasketDto = _mapper.Map<UserBasketDto>(user);

            userBasketDto.BasketGames = _mapper.Map<List<BasketGameDto>>(basket.BasketGames);

            return userBasketDto;
        }


        public async Task<Basket> Add(string email, int gameId, int quantity)
        {
            var user = await _unitOfWork.userRepository.GetEntity(u => u.Email == email);
            if (user == null) throw new Exception("User not found.");

            // Include BasketGames in the basket
            var basket = await _unitOfWork.basketRepository.GetEntity(b => b.UserId == user.Id, "BasketGames.Game");

            if (basket == null)
            {
                basket = new Basket { UserId = user.Id, BasketGames = new List<BasketGame>() };
                await _unitOfWork.basketRepository.Create(basket);
                _unitOfWork.Commit();
            }

            var basketGame = basket.BasketGames.FirstOrDefault(bg => bg.GameId == gameId);

            if (basketGame != null)
            {
                basketGame.Quantity += quantity;
                await _unitOfWork.basketGameRepository.Update(basketGame);
            }
            else
            {
                var newBasketGame = new BasketGame
                {
                    BasketId = basket.Id,
                    GameId = gameId,
                    Quantity = quantity
                };
                await _unitOfWork.basketGameRepository.Create(newBasketGame);
            }

            _unitOfWork.Commit();
            return basket;
        }

        public async Task<Basket> Update(string email, int gameId, int quantity)
        {
            var user = await _unitOfWork.userRepository.GetEntity(u => u.Email == email);
            if (user == null) throw new Exception("User not found.");

            // Include BasketGames in the basket
            var basket = await _unitOfWork.basketRepository.GetEntity(b => b.UserId == user.Id, "BasketGames.Game");

            if (basket == null) throw new Exception("Basket not found.");

            var basketGame = basket.BasketGames.FirstOrDefault(bg => bg.GameId == gameId);

            if (basketGame != null)
            {
                basketGame.Quantity = quantity;
                await _unitOfWork.basketGameRepository.Update(basketGame);
                _unitOfWork.Commit();
            }
            else
            {
                throw new Exception("Game not found in basket.");
            }

            return basket;
        }

        public async Task<bool> Delete(string email, int gameId)
        {
            var user = await _unitOfWork.userRepository.GetEntity(u => u.Email == email);
            if (user == null) throw new Exception("User not found.");

            // Include BasketGames in the basket
            var basket = await _unitOfWork.basketRepository.GetEntity(b => b.UserId == user.Id, "BasketGames.Game");
            if (basket == null) throw new Exception("Basket not found.");

            var basketGame = basket.BasketGames.FirstOrDefault(bg => bg.GameId == gameId);
            if (basketGame != null)
            {
                basket.BasketGames.Remove(basketGame);
                await _unitOfWork.basketGameRepository.Delete(basketGame);
                _unitOfWork.Commit();
                return true;
            }

            return false;
        }

        public async Task<bool> ClearBasket(string email)
        {
            var user = await _unitOfWork.userRepository.GetEntity(u => u.Email == email);
            if (user == null) throw new Exception("User not found.");

            // Include BasketGames in the basket
            var basket = await _unitOfWork.basketRepository.GetEntity(b => b.UserId == user.Id, "BasketGames.Game");
            if (basket == null) throw new Exception("Basket not found.");

            foreach (var basketGame in basket.BasketGames.ToList())
            {
                await _unitOfWork.basketGameRepository.Delete(basketGame);
            }

            _unitOfWork.Commit();
            return true;
        }


    }
}
