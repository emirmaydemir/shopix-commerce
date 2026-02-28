using AutoMapper;
using shopix_commerce_core.ApplicationServices.Interfaces;
using shopix_commerce_core.DTO.Category;
using shopix_commerce_infrastructure.Models;
using shopix_commerce_infrastructure.UoW;
using shopix_core_domain.Entities;

namespace shopix_commerce_core.ApplicationServices.Concretes
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<ResponseModel<CategoryDTO>> CreateCategory(CreateCategoryDTO categoryDTO)
        {
            var category = _mapper.Map<Category>(categoryDTO);
            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveAsync();
            var categoryData = _mapper.Map<CategoryDTO>(category);
            return new ResponseModel<CategoryDTO>
            {
                Data = categoryData,
                IsSuccess = true,
                Message = "Category created successfully."
            };
        }

        public async Task<ResponseModel<bool>> DeleteCategory(Guid Id)
        {
            var category = _unitOfWork.Categories.GetByIdAsync(Id);
            if (category == null) 
            {
                return new ResponseModel<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    Message = "Category not found."
                };
            }
            await _unitOfWork.Categories.SoftDeleteAsync(Id);
            await _unitOfWork.SaveAsync();
            return new ResponseModel<bool>
            {
                Data = true,
                IsSuccess = true,
                Message = "Category deleted successfully."
            };
        }

        public async Task<ResponseModel<IEnumerable<CategoryDTO>>> GetCategories()
        {
            IEnumerable<Category> categories = await _unitOfWork.Categories.GetAllAsync();
            var categoryDTOs = _mapper.Map<IEnumerable<CategoryDTO>>(categories);
            return new ResponseModel<IEnumerable<CategoryDTO>>
            {
                Data = categoryDTOs,
                IsSuccess = true,
                Message = "Categories retrieved successfully."
            };
        }

        public async Task<ResponseModel<CategoryDTO>> GetCategory(Guid Id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(Id);
            if (category == null)
            {
                return new ResponseModel<CategoryDTO>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = "Category not found."
                };
            }
            var categoryDTO = _mapper.Map<CategoryDTO>(category);
            return new ResponseModel<CategoryDTO>
            {
                Data = categoryDTO,
                IsSuccess = true,
                Message = "Category retrieved successfully."
            };
        }

        public async Task<ResponseModel<CategoryDTO>> UpdateCategory(Guid Id, UpdateCategoryDTO categoryDTO)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(Id);
            if (category == null)
            {
                return new ResponseModel<CategoryDTO>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = "Category not found."
                };
            }
            _mapper.Map(categoryDTO, category);
            await _unitOfWork.Categories.UpdateAsync(category);
            await _unitOfWork.SaveAsync();
            var updatedCategoryDTO = _mapper.Map<CategoryDTO>(category);
            return new ResponseModel<CategoryDTO>
            {
                Data = updatedCategoryDTO,
                IsSuccess = true,
                Message = "Category updated successfully."
            };
        }
    }
}
