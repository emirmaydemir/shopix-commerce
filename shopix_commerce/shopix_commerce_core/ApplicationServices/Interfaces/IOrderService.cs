using shopix_commerce_core.DTO.Order;
using shopix_commerce_infrastructure.Models;

namespace shopix_commerce_core.ApplicationServices.Interfaces
{
    public interface IOrderService
    {
        Task<ResponseModel<IEnumerable<OrderDTO>>> GetOrders();
        Task<ResponseModel<OrderDTO>> GetOrderById(Guid id);
        Task<ResponseModel<OrderDTO>> CreateOrder(CreateOrderDTO createOrderDto);
    }
}
