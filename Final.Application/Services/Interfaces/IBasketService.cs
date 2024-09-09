using Final.Core.Entities;

namespace Final.Application.Services.Interfaces
{
    public interface IBasketService
    {
        Task<Basket> GetBasketByEmail(string email);
        Task<Basket> Add(string email, int gameId, int quantity);
        Task<Basket> Update(string email, int gameId, int quantity);
        Task<bool> Delete(string email, int gameId);
        Task<bool> ClearBasket(string email);
    }
}
