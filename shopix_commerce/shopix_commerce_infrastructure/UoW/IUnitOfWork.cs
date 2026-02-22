using shopix_core_domain.Entities;
using shopix_core_domain.Interfaces.Repository;

namespace shopix_commerce_infrastructure.UoW
{
    public interface IUnitOfWork
    {
        IGenericRepository<Product> Products { get; }
        IGenericRepository<Category> Categories { get; }
        IGenericRepository<Address> Addresses { get; }
        IGenericRepository<Order> Orders { get; }
        IGenericRepository<OrderItem> OrderItems { get; }
        IGenericRepository<ProductImage> ProductImages { get; }
        Task<int> SaveAsync();
    }
}
