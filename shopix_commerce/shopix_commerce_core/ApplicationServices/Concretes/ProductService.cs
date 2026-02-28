using AutoMapper;
using Microsoft.EntityFrameworkCore;
using shopix_commerce_core.ApplicationServices.Interfaces;
using shopix_commerce_core.DTO.Product;
using shopix_commerce_infrastructure.Models;
using shopix_commerce_infrastructure.UoW;
using shopix_core_domain.Entities;

namespace shopix_commerce_core.ApplicationServices.Concretes
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<ResponseModel<ProductImageDTO>> AddProductImage(Guid Id, AddProductImageDTO addImageDTO)
        {
            var product = await _unitOfWork.Products.GetAllAsyncWithInclude(
                predicate: x => x.Id == Id,
                include: p => p.Include(pr => pr.ProductImages)
                );

            if (product is null)
            {
                return new ResponseModel<ProductImageDTO>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = "Product not found"
                };
            }

            if (addImageDTO.Image == null || addImageDTO.Image.Length == 0)
            {
                return new ResponseModel<ProductImageDTO>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = "Invalid image file"
                };
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(addImageDTO.Image.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                return new ResponseModel<ProductImageDTO>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = "Unsupported image format"
                };
            }

            var fileName = $"{Guid.NewGuid()}{extension}";
            var imagesFolder = Path.Combine("wwwroot", "images");
            if (!Directory.Exists(imagesFolder))
            {
                Directory.CreateDirectory(imagesFolder);
            }

            var filePath = Path.Combine(imagesFolder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await addImageDTO.Image.CopyToAsync(stream);
            }

            if (addImageDTO.IsMain)
            {
                var currentMainImage = product.FirstOrDefault().ProductImages.FirstOrDefault(img => img.IsMain);
                if (currentMainImage != null)
                {
                    currentMainImage.IsMain = false;
                    await _unitOfWork.ProductImages.UpdateAsync(currentMainImage);
                    await _unitOfWork.SaveAsync();

                }

            }

            var productImage = new ProductImage
            {
                ProductId = Id,
                ImageUrl = $"/images/{fileName}",
                IsMain = addImageDTO.IsMain
            };

            await _unitOfWork.ProductImages.AddAsync(productImage);
            await _unitOfWork.SaveAsync();
            var productImageDto = _mapper.Map<ProductImageDTO>(productImage);
            return new ResponseModel<ProductImageDTO>
            {
                Data = productImageDto,
                IsSuccess = true,
                Message = "Image added successfully"
            };
        }

        public async Task<ResponseModel<bool>> DeleteProductImage(Guid productId, Guid imageId)
        {
            var image = await _unitOfWork.ProductImages.FindAsync(x => x.Id == imageId);
            if (image == null)
            {
                return new ResponseModel<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    Message = "Image not found"
                };
            }
            if (!string.IsNullOrEmpty(image.FirstOrDefault().ImageUrl))
            {
                var fileName = Path.GetFileName(image.FirstOrDefault().ImageUrl);
                var filePath = Path.Combine("wwwroot", "images", fileName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }

            await _unitOfWork.ProductImages.SoftDeleteAsync(imageId);
            await _unitOfWork.SaveAsync();
            return new ResponseModel<bool>
            {
                Data = true,
                IsSuccess = true,
                Message = "Image deleted successfully"
            };
        }

        public async Task<ResponseModel<ProductDTO>> CreateProduct(CreateProductDTO createProductDTO)
        {
            var productMap = _mapper.Map<Product>(createProductDTO);
            await _unitOfWork.Products.AddAsync(productMap);
            await _unitOfWork.SaveAsync();
            var productDTO = _mapper.Map<ProductDTO>(productMap);
            return new ResponseModel<ProductDTO>
            {
                Data = productDTO,
                IsSuccess = true,
                Message = "Product created successfully"
            };
        }

        public async Task<ResponseModel<bool>> DeleteProduct(Guid Id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(Id);
            if (product == null)
            {
                return new ResponseModel<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    Message = "Product not found"
                };
            }

            await _unitOfWork.Products.SoftDeleteAsync(Id);
            await _unitOfWork.SaveAsync();
            return new ResponseModel<bool>
            {
                Data = true,
                IsSuccess = true,
                Message = "Product deleted successfully"
            };
        }

        public async Task<ResponseModel<IEnumerable<ProductDTO>>> GetProducts(Guid categoryId)
        {
            if (categoryId.ToString().StartsWith("000"))
            {
                var products = await _unitOfWork.Products.GetAllAsyncWithInclude(
                 null,
                 include: p => p.Include(pr => pr.ProductImages).Include(x => x.Category)
             );
                var productsDTO = _mapper.Map<IEnumerable<ProductDTO>>(products);
                var response = new ResponseModel<IEnumerable<ProductDTO>>
                {
                    Data = productsDTO,
                    IsSuccess = true,
                    Message = "Products retrieved successfully"
                };
                return response;
            }
            else
            {
                var products = await _unitOfWork.Products.GetAllAsyncWithInclude(
               predicate: p => p.CategoryId == categoryId,
               include: p => p.Include(pr => pr.ProductImages)
           );
                var productsDTO = _mapper.Map<IEnumerable<ProductDTO>>(products);
                var response = new ResponseModel<IEnumerable<ProductDTO>>
                {
                    Data = productsDTO,
                    IsSuccess = true,
                    Message = "Products retrieved successfully"
                };
                return response;
            }
        }

        public async Task<ResponseModel<ProductDTO>> GetProductById(Guid Id)
        {
            var products = await _unitOfWork.Products.GetAllAsyncWithInclude(
                predicate: p => p.Id == Id,
                include: p => p.Include(pr => pr.ProductImages).Include(pr => pr.Category)
            );
            var product = products.FirstOrDefault();
            var productDTO = _mapper.Map<ProductDTO>(product);
            return new ResponseModel<ProductDTO>
            {
                Data = productDTO,
                IsSuccess = true,
                Message = "Product retrieved successfully."
            };
        }

        public async Task<ResponseModel<ProductDTO>> UpdateProduct(Guid Id, UpdateProductDTO updateProductDTO)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(Id);
            if (product == null)
            {
                return new ResponseModel<ProductDTO>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = "Product not found"
                };
            }
            _mapper.Map(updateProductDTO, product);
            await _unitOfWork.Products.UpdateAsync(product);
            await _unitOfWork.SaveAsync();
            var productDTO = _mapper.Map<ProductDTO>(product);
            return new ResponseModel<ProductDTO>
            {
                Data = productDTO,
                IsSuccess = true,
                Message = "Product updated successfully"
            };
        }
    }
}
