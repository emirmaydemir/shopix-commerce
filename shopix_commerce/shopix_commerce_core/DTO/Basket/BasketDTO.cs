namespace shopix_commerce_core.DTO.Basket
{
    public class BasketDTO
    {
        public string Id { get; set; } = string.Empty;
        public List<BasketItemDTO> Items { get; set; } = new List<BasketItemDTO>();
        public decimal Total => Items.Sum(item => item.Quantity * item.Price);

    }

    public class BasketItemDTO
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }

    public class AddItemToBasketDTO
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
