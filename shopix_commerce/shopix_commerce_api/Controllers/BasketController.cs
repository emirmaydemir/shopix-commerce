using Microsoft.AspNetCore.Mvc;
using shopix_commerce_core.ApplicationServices.Interfaces;
using shopix_commerce_core.DTO.Basket;

namespace shopix_commerce_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketService _basketService;

        public BasketController(IBasketService basketService)
        {
            _basketService = basketService;
        }

        [HttpDelete("{basketId}/items/{productId}")]
        public async Task<IActionResult> RemoveItemFromUniqeBasket(string basketId, Guid productId)
        {
            var basket = await _basketService.RemoveItemFromBasket(basketId, productId);
            if (!basket.IsSuccess)
                return NotFound(basket.Message);
            return Ok(basket);
        }

        [HttpGet("{basketId}")]
        public async Task<IActionResult> GetBasket(string basketId)
        {
            var basket = await _basketService.GetBasketAsync(basketId);
            if (basket == null)
                return NotFound("Basket not found.");

            return Ok(basket);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> UpdateBasket(string id, AddItemToBasketDTO addItemDto)
        {

            var updatedBasket = await _basketService.UpdateBasketAsync(id, addItemDto);
            return updatedBasket.IsSuccess ? Ok(updatedBasket) : BadRequest(updatedBasket);
        }

        [HttpDelete("{basketId}")]
        public async Task<IActionResult> DeleteBasket(string basketId)
        {
            var result = await _basketService.DeleteBasketAsync(basketId);

            if (!result.IsSuccess)
                return NotFound("Basket not found.");

            return Ok(new { message = "Basket deleted successfully." });
        }
    }
}
