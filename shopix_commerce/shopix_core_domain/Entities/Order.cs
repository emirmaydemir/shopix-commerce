using System.ComponentModel.DataAnnotations.Schema;

namespace shopix_core_domain.Entities
{
    public class Order : BaseEntity
    {
        public string OrderNumber { get; set; } = string.Empty;

        //Address
        public string ShippingFullName { get; set; } = string.Empty;
        public string ShippingPhoneNumber { get; set; } = string.Empty;
        public string ShippingCity { get; set; } = string.Empty;
        public string ShippingDistrict { get; set; } = string.Empty;
        public string ShippingAddressLine { get; set; } = string.Empty;
        public string? ShippingZipCode { get; set; }

        //Payment Info
        public decimal TotalAmount { get; set; }
        public string PaymentId { get; set; } = string.Empty;
        public string OrderStatus { get; set; } = "Processing";
        public string PaymentStatus { get; set; } = "Pending";
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }
        public virtual ApplicationUser User { get; set; } = null!;
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
