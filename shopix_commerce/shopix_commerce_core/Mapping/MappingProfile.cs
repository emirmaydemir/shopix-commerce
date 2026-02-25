using AutoMapper;
using shopix_commerce_core.DTO.Category;
using shopix_core_domain.Entities;

namespace shopix_commerce_core.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region Category
            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<Category, UpdateCategoryDTO>().ReverseMap();
            CreateMap<Category, CreateCategoryDTO>().ReverseMap();
            #endregion
        }
    }
}
