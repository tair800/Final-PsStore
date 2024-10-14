using AutoMapper;
using Final.Application.Dtos.OrderDtos;
using Final.Application.Exceptions;
using Final.Application.Services.Interfaces;
using Final.Core.Entities;
using Final.Data.Implementations;

namespace Final.Application.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<int> CreateOrder(string userId)
        {
            // Fetch the user's basket with related games
            var basket = await _unitOfWork.basketRepository.GetEntity(b => b.UserId == userId, "BasketGames.Game");
            if (basket == null || !basket.BasketGames.Any())
            {
                throw new CustomExceptions(400, "Basket", "Basket is empty or does not exist.");
            }

            // Create the order entity
            var order = new Order
            {
                UserId = userId,
                OrderItems = basket.BasketGames.Select(basketGame => new OrderItem
                {
                    GameId = basketGame.GameId,
                    Quantity = basketGame.Quantity,
                    Price = (decimal)basketGame.Game.Price // Keep Price as decimal
                }).ToList(),
                TotalPrice = (decimal)basket.BasketGames.Sum(b => b.Game.Price * b.Quantity), // Sum prices as decimal
                CreatedDate = DateTime.UtcNow
            };
            if (order is null) throw new CustomExceptions(400, "order", "order is null");
            // Add the order to the database
            await _unitOfWork.orderRepository.Create(order);

            _unitOfWork.Commit();  // Ensure this saves both the order and its items

            return order.Id;
        }

        // Get order by ID
        public async Task<OrderDto> GetOrder(int orderId)
        {
            // Fetch the order with related OrderItems and their corresponding Game
            var order = await _unitOfWork.orderRepository
                .GetEntity(o => o.Id == orderId, "OrderItems.Game");

            if (order == null)
            {
                throw new CustomExceptions(404, "Order", "Order not found.");
            }

            return _mapper.Map<OrderDto>(order);
        }

    }
}
