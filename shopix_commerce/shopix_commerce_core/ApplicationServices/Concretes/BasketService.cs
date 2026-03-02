using AutoMapper;
using Microsoft.EntityFrameworkCore;
using shopix_commerce_core.ApplicationServices.Interfaces;
using shopix_commerce_core.DTO.Basket;
using shopix_commerce_infrastructure.Models;
using shopix_commerce_infrastructure.UoW;
using shopix_core_domain.Entities;
using shopix_core_domain.Interfaces.Repository;

namespace shopix_commerce_core.ApplicationServices.Concretes
{
    public class BasketService : IBasketService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInMemoryRepository _memoryRepository;
        public BasketService(IMapper mapper, IUnitOfWork unitOfWork, IInMemoryRepository memoryRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _memoryRepository = memoryRepository;      
        }

        public async Task<ResponseModel<bool>> DeleteBasketAsync(string basketId)
        {
            await _memoryRepository.DeleteBasketAsync(basketId);
            return new ResponseModel<bool>
            {
                Data = true,
                IsSuccess = true,
                Message = "Basket deleted successfully."
            };
        }

        public async Task<ResponseModel<BasketDTO?>> GetBasketAsync(string basketId)
        {
            var basket = await _memoryRepository.GetBasketAsync(basketId);
            if (basket == null)
            {
                return new ResponseModel<BasketDTO?>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = "Basket not found."
                };
            }
            var basketDto = _mapper.Map<BasketDTO>(basket);
            return new ResponseModel<BasketDTO?>
            {
                Data = basketDto,
                IsSuccess = true,
                Message = "Basket retrieved successfully."
            };

        }

        public async Task<ResponseModel<BasketDTO>> RemoveItemFromBasket(string basketId, Guid productId)
        {
            var basket = await _memoryRepository.GetBasketAsync(basketId);
            if (basket == null)
            {
                return new ResponseModel<BasketDTO>
                {
                    Data = null!,
                    IsSuccess = false,
                    Message = "Basket not found."
                };
            }
            basket.Items.RemoveAll(x => x.ProductId == productId.ToString());
            var updatedBasket = await _memoryRepository.UpdateBasketAsync(basket);
            var basketDto = _mapper.Map<BasketDTO>(updatedBasket);
            return new ResponseModel<BasketDTO>
            {
                Data = basketDto,
                IsSuccess = true,
                Message = "Item removed from basket successfully."
            };

        }

        public async Task<ResponseModel<BasketDTO?>> UpdateBasketAsync(string basketId, AddItemToBasketDTO basketDto)
        {
            var basket = await _memoryRepository.GetBasketAsync(basketId) ?? new Basket { Id = basketId };

            var product = await _unitOfWork.Products.GetByIdAsyncExpressionWithInclude(x => x.Id == Guid.Parse(basketDto.ProductId), x => x.Include(x => x.ProductImages));

            if (product == null)
            {
                return new ResponseModel<BasketDTO?>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = "Product not found."
                };
            }

            var basketItem = basket.Items.FirstOrDefault(x => x.ProductId == basketDto.ProductId);
            if (basketItem != null)
            {
                basketItem.Quantity += basketDto.Quantity;
            }
            else
            {
                basket.Items.Add(new BasketItem
                {
                    ProductId = basketDto.ProductId,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = basketDto.Quantity,
                    ImageUrl = product.ProductImages.FirstOrDefault()?.ImageUrl ?? string.Empty
                });
            }
            var updatedBasket = await _memoryRepository.UpdateBasketAsync(basket);
            var updatedBasketDto = _mapper.Map<BasketDTO>(updatedBasket);
            return new ResponseModel<BasketDTO?>
            {
                Data = updatedBasketDto,
                IsSuccess = true,
                Message = "Basket updated successfully."
            };
        }
    }
}
