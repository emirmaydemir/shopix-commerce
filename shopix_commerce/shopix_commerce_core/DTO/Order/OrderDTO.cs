namespace shopix_commerce_core.DTO.Order
{
    public class OrderDTO
    {
        public Guid Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string ShippingFullName { get; set; } = string.Empty;
        public string ShippingPhone { get; set; } = string.Empty;
        public string ShippingCity { get; set; } = string.Empty;
        public string ShippingDistrict { get; set; } = string.Empty;
        public string ShippingAddressLine { get; set; } = string.Empty;
        public string? ShippingZipCode { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public string OrderStatus { get; set; } = string.Empty;
        public string PaymentId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public ICollection<OrderItemDTO> OrderItems { get; set; } = new List<OrderItemDTO>();
    }

    public class OrderItemDTO
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class CreateOrderDTO
    {
        public Guid AddressId { get; set; }
        public List<CreateOrderItemDTO> Items { get; set; } = new List<CreateOrderItemDTO>();
        public PaymentCardDTO Card { get; set; } = null!;
    }

    public class CreateOrderItemDTO
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class PaymentCardDTO
    {
        public string CardHolderName { get; set; } = string.Empty;
        public string CardNumber { get; set; } = string.Empty;
        public string ExpireMonth { get; set; } = string.Empty;
        public string ExpireYear { get; set; } = string.Empty;
        public string Cvc { get; set; } = string.Empty;
    }
}
