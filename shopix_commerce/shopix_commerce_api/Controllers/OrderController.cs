using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using shopix_commerce_core.ApplicationServices.Interfaces;
using shopix_commerce_core.DTO.Order;

namespace shopix_commerce_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDTO orderDTO)
        {
            var result = await _orderService.CreateOrder(orderDTO);
            if (!result.IsSuccess)
                return BadRequest(result.Message);
            return CreatedAtAction(nameof(GetOrderById), new { id = result.Data.Id }, result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _orderService.GetOrders();
            if (!orders.IsSuccess)
                return NotFound(orders.Message);
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var order = await _orderService.GetOrderById(id);
            if (!order.IsSuccess)
                return NotFound(order.Message);
            return Ok(order);
        }
    }
}
