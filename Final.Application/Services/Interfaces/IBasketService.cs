using Final.Application.Dtos.BasketDtos;
using Final.Core.Entities;

namespace Final.Application.Services.Interfaces
{
    public interface IBasketService
    {
        Task<UserBasketDto> GetBasketByUser(string userId);
        Task<Basket> Add(string userId, int gameId, int quantity);
        Task<Basket> Update(string userId, int gameId, int quantity);
        Task<bool> Delete(string userId, int gameId);
        Task<bool> ClearBasket(string userId);
        Task<Basket> AddDlc(string userId, int dlcId, int quantity);
        Task<bool> DeleteDlc(string userId, int dlcId);


    }
}
