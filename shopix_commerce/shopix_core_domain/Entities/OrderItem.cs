using System.ComponentModel.DataAnnotations.Schema;

namespace shopix_core_domain.Entities
{
    public class OrderItem : BaseEntity
    {
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal => Price * Quantity;

        [ForeignKey(nameof(Order))]
        public Guid OrderId { get; set; }
        public virtual Order Order { get; set; } = null!;
        [ForeignKey(nameof(Product))]
        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;
    }
}
