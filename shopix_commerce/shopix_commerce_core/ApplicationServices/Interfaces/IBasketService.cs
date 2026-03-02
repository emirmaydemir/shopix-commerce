using shopix_commerce_core.DTO.Basket;
using shopix_commerce_infrastructure.Models;

namespace shopix_commerce_core.ApplicationServices.Interfaces
{
    public interface IBasketService
    {
        Task<ResponseModel<BasketDTO?>> GetBasketAsync(string basketId);
        Task<ResponseModel<BasketDTO?>> UpdateBasketAsync(string basketId, AddItemToBasketDTO basketDto);
        Task<ResponseModel<bool>> DeleteBasketAsync(string basketId);
        Task<ResponseModel<BasketDTO>> RemoveItemFromBasket(string basketId, Guid productId);
    }
}
