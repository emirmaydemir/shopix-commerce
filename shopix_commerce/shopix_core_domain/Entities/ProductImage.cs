using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace shopix_core_domain.Entities
{
    public class ProductImage : BaseEntity  
    {
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsMain { get; set; }
        [ForeignKey(nameof(Product))]
        public Guid ProductId { get; set; }
        [JsonIgnore]
        public virtual Product Product { get; set; } = null!;
    }
}
