using shopix_commerce_core.DTO.Product;
using shopix_commerce_infrastructure.Models;

namespace shopix_commerce_core.ApplicationServices.Interfaces
{
    public interface IProductService
    {
        Task<ResponseModel<IEnumerable<ProductDTO>>> GetProducts(Guid categoryId);
        Task<ResponseModel<ProductDTO>> GetProductById(Guid Id);
        Task<ResponseModel<ProductDTO>> CreateProduct(CreateProductDTO createProductDTO);
        Task<ResponseModel<ProductDTO>> UpdateProduct(Guid Id, UpdateProductDTO updateProductDTO);
        Task<ResponseModel<bool>> DeleteProduct(Guid Id);
        Task<ResponseModel<ProductImageDTO>> AddProductImage(Guid Id, AddProductImageDTO addImageDTO);
        Task<ResponseModel<bool>> DeleteProductImage(Guid productId, Guid imageId);
    }
}
