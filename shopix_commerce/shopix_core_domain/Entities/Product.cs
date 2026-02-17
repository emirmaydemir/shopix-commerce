using System.ComponentModel.DataAnnotations.Schema;

namespace shopix_core_domain.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        [ForeignKey(nameof(Category))]
        public Guid CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;
        public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    }
}
