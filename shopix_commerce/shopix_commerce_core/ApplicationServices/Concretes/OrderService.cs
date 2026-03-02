using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using shopix_commerce_core.ApplicationServices.Interfaces;
using shopix_commerce_core.DTO.Order;
using shopix_commerce_infrastructure.CurrentUser;
using shopix_commerce_infrastructure.Models;
using shopix_commerce_infrastructure.PaymentService;
using shopix_commerce_infrastructure.UoW;
using shopix_core_domain.Entities;

namespace shopix_commerce_core.ApplicationServices.Concretes
{
    public class OrderService : IOrderService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContext _userContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPaymentService _paymentService;
        public OrderService(IMapper mapper, IUnitOfWork unitOfWork, IUserContext userContext, UserManager<ApplicationUser> userManager, IPaymentService paymentService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userContext = userContext;
            _userManager = userManager;
            _paymentService = paymentService;
        }

        public async Task<ResponseModel<OrderDTO>> CreateOrder(CreateOrderDTO createOrderDto)
        {
            var user = await _userManager.FindByIdAsync(_userContext.UserId!);
            if (user is null)
            {
                return new ResponseModel<OrderDTO>
                {
                    IsSuccess = false,
                    Message = "User not found."
                };
            }

            var adress = await _unitOfWork.Addresses.GetByIdAsync(createOrderDto.AddressId);
            if (adress is null || adress.UserId != _userContext.UserId)
            {
                return new ResponseModel<OrderDTO>
                {
                    IsSuccess = false,
                    Message = "Address not found."
                };
            }

            var productIds = createOrderDto.Items.Select(i => i.ProductId).ToList();

            var products = await _unitOfWork.Products.FindAsync(
                predicate: x => productIds.Contains(x.Id)
            );

            if (products.Count() != productIds.Count)
            {
                return new ResponseModel<OrderDTO>
                {
                    IsSuccess = false,
                    Message = "One or more products not found."
                };
            }

            foreach (var item in createOrderDto.Items)
            {
                var product = products.First(x => x.Id == item.ProductId);
                if (item.Quantity > product.Stock)
                {
                    return new ResponseModel<OrderDTO>
                    {
                        IsSuccess = false,
                        Message = $"Insufficient stock for product {product.Name}."
                    };
                }
            }

            //total amount

            decimal totalAmount = 0;
            var orderItems = new List<OrderItem>();
            foreach (var item in createOrderDto.Items)
            {
                var product = products.First(x => x.Id == item.ProductId);
                totalAmount += product.Price * item.Quantity;
                orderItems.Add(new OrderItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Quantity = item.Quantity,
                    Price = product.Price
                });
            }

            var orderNumber = $"SHOPIX_ORD{DateTime.UtcNow:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";

            var paymentRequest = new PaymentRequest
            {
                OrderNumber = orderNumber,
                Amount = totalAmount,
                Card = new shopix_commerce_infrastructure.PaymentService.PaymentCardDTO
                {
                    CardHolderName = createOrderDto.Card.CardHolderName,
                    CardNumber = createOrderDto.Card.CardNumber,
                    ExpireMonth = createOrderDto.Card.ExpireMonth,
                    ExpireYear = createOrderDto.Card.ExpireYear,
                    Cvc = createOrderDto.Card.Cvc
                },
                Buyer = new BuyerInfo
                {
                    Id = _userContext.UserId,
                    Name = user.FirstName,
                    Surname = user.LastName,
                    Email = user.Email!,
                    RegistrationAddress = adress.AddressLine,
                    City = adress.City
                },
                ShippingAddress = new AddressInfo
                {
                    ContactName = adress.FullName,
                    City = adress.City,
                    Address = adress.AddressLine
                },
                Items = orderItems.Select(oi => new BasketItemInfo
                {
                    Id = oi.ProductId.ToString(),
                    Name = oi.ProductName,
                    Price = oi.Price * oi.Quantity
                }).ToList()
            };

            var paymentResult = await _paymentService.ProcessPaymentAsync(paymentRequest);

            if (!paymentResult.Success)
            {
                return new ResponseModel<OrderDTO>
                {
                    IsSuccess = false,
                    Message = $"Payment failed: {paymentResult.ErrorMessage}"
                };
            }

            var order = new Order
            {
                OrderNumber = orderNumber,
                UserId = _userContext.UserId,
                ShippingFullName = adress.FullName,
                ShippingPhoneNumber = adress.PhoneNumber,
                ShippingCity = adress.City,
                ShippingDistrict = adress.District,
                ShippingAddressLine = adress.AddressLine,
                ShippingZipCode = adress.ZipCode,
                TotalAmount = totalAmount,
                PaymentId = paymentResult.PaymentId,
                PaymentStatus = "Completed",
                OrderStatus = "Processing",
                OrderItems = orderItems,
            };

            await _unitOfWork.Orders.AddAsync(order);

            foreach (var item in createOrderDto.Items)
            {
                var product = products.First(x => x.Id == item.ProductId);
                product.Stock -= item.Quantity;
                product.UpdatedAt = DateTime.UtcNow;
            }

            await _unitOfWork.SaveAsync();

            var orderDTO = await _unitOfWork.Orders.GetByIdAsyncExpressionWithInclude(
                predicate: x => x.Id == order.Id,
                include: x => x.Include(y => y.OrderItems)
                );

            var orderMap = _mapper.Map<OrderDTO>(orderDTO);

            return new ResponseModel<OrderDTO>
            {
                IsSuccess = true,
                Data = orderMap
            };

        }

        public async Task<ResponseModel<OrderDTO>> GetOrderById(Guid id)
        {
            var orders = await _unitOfWork.Orders.GetByIdAsyncExpressionWithInclude(
                predicate: x => x.Id == id && x.UserId == _userContext.UserId, 
                include: x => x.Include(y => y.OrderItems)
            );

            if (orders is null)
            {
                return new ResponseModel<OrderDTO>
                {
                    IsSuccess = false,
                    Message = "Order not found."
                };
            }

            var orderDto = _mapper.Map<OrderDTO>(orders);
            return new ResponseModel<OrderDTO>
            {
                IsSuccess = true,
                Data = orderDto
            };

        }

        public async Task<ResponseModel<IEnumerable<OrderDTO>>> GetOrders()
        {
            var orders = await _unitOfWork.Orders.GetAllAsyncWithInclude(
                predicate: x => x.UserId == _userContext.UserId,
                include: x => x.Include(y => y.OrderItems)              
            );

            var userOrders = orders.OrderByDescending(x => x.CreatedAt).ToList();

            if (userOrders.Count == 0)
            {
                return new ResponseModel<IEnumerable<OrderDTO>>
                {
                    IsSuccess = false,
                    Message = "No orders found."
                };
            }

            var ordersDto = _mapper.Map<IEnumerable<OrderDTO>>(userOrders);
            return new ResponseModel<IEnumerable<OrderDTO>>
            {
                IsSuccess = true,
                Data = ordersDto
            };
        }
    }
}
