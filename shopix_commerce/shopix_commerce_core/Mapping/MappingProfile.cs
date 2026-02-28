using AutoMapper;
using shopix_commerce_core.DTO.Category;
using shopix_commerce_core.DTO.Product;
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

            #region Product
            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ProductImages)).ReverseMap();

            CreateMap<Product, CreateProductDTO>().ReverseMap();
            CreateMap<Product, UpdateProductDTO>().ReverseMap();
            #endregion

            #region ProductImage
            CreateMap<ProductImage, ProductImageDTO>().ReverseMap();
            CreateMap<ProductImage, AddProductImageDTO>().ReverseMap();
            #endregion
        }
    }
}
