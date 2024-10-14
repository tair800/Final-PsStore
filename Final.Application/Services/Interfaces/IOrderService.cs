using Final.Application.Dtos.OrderDtos;

namespace Final.Application.Services.Interfaces
{
    public interface IOrderService
    {
        Task<int> CreateOrder(string userId);
        Task<OrderDto> GetOrder(int orderId);
    }
}
