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
            var basket = await _unitOfWork.basketRepository.GetEntity(b => b.UserId == userId, "BasketGames.Game", "OrderItems.Game");
            if (basket == null || !basket.BasketGames.Any())
            {
                throw new CustomExceptions(400, "Basket", "Basket is empty or does not exist.");
            }

            // Use AutoMapper to map BasketGames to OrderItems
            var order = new Order
            {
                UserId = userId,
                OrderItems = _mapper.Map<List<OrderItem>>(basket.BasketGames),
                TotalPrice = (decimal)basket.BasketGames.Sum(b => b.Game.Price * b.Quantity), // Sum prices as decimal
                CreatedDate = DateTime.UtcNow
            };

            if (order == null) throw new CustomExceptions(400, "Order", "Order is null.");

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

        public async Task<OrderReturnDto> Checkout(string userId)
        {
            // Fetch user's basket
            var basket = await _unitOfWork.basketRepository.GetEntity(b => b.UserId == userId, "BasketGames.Game");
            if (basket == null || !basket.BasketGames.Any())
            {
                throw new CustomExceptions(400, "Basket is empty. Cannot proceed to checkout.");
            }

            // Create a new order
            var order = new Order
            {
                UserId = userId,
                OrderItems = new List<OrderItem>(),
                TotalPrice = 0,
                CreatedDate = DateTime.UtcNow
            };

            // Add each game in the basket to the order
            foreach (var basketGame in basket.BasketGames)
            {
                var orderItem = new OrderItem
                {
                    GameId = basketGame.GameId,
                    Quantity = basketGame.Quantity,
                    Price = (int)basketGame.Game.Price * basketGame.Quantity
                };
                order.OrderItems.Add(orderItem);
                order.TotalPrice += orderItem.Price;
            }

            // Save the order to the database
            await _unitOfWork.orderRepository.Create(order);
            _unitOfWork.Commit();

            // Clear the user's basket after the checkout
            await _unitOfWork.basketRepository.Delete(basket);
            _unitOfWork.Commit();

            // Fetch the saved order with its order items
            var savedOrder = await _unitOfWork.orderRepository.GetEntity(o => o.Id == order.Id, "OrderItems.Game");

            // Map to OrderDto
            var orderDto = _mapper.Map<OrderDto>(savedOrder);

            // Map OrderDto to OrderReturnDto
            var orderReturnDto = _mapper.Map<OrderReturnDto>(orderDto);

            return orderReturnDto;
        }

    }
}
