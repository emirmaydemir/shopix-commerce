using shopix_commerce_core.DTO.Category;
using shopix_commerce_infrastructure.Models;

namespace shopix_commerce_core.ApplicationServices.Interfaces
{
    public interface ICategoryService
    {
        Task<ResponseModel<IEnumerable<CategoryDTO>>> GetCategories();
        Task<ResponseModel<CategoryDTO>> GetCategory(Guid Id);
        Task<ResponseModel<CategoryDTO>> CreateCategory(CreateCategoryDTO categoryDTO);
        Task<ResponseModel<CategoryDTO>> UpdateCategory(Guid Id, UpdateCategoryDTO categoryDTO);
        Task<ResponseModel<bool>> DeleteCategory(Guid Id);
    }
}
