using System.ComponentModel.DataAnnotations.Schema;

namespace shopix_core_domain.Entities
{
    public class ProductImage : BaseEntity  
    {
        public string Url { get; set; } = string.Empty;
        public bool IsMain { get; set; }
        [ForeignKey(nameof(Product))]
        public Guid ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;
    }
}
