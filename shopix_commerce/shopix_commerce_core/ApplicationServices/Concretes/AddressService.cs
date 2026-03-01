using AutoMapper;
using Microsoft.AspNetCore.Http;
using shopix_commerce_core.ApplicationServices.Interfaces;
using shopix_commerce_core.DTO.Address;
using shopix_commerce_infrastructure.Models;
using shopix_commerce_infrastructure.UoW;
using shopix_core_domain.Entities;
using System.Security.Claims;

namespace shopix_commerce_core.ApplicationServices.Concretes
{
    public class AddressService : IAddressService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly string? _userId;
        public AddressService(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userId = httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
        public async Task<ResponseModel<AddressDTO>> CreateAddressAsync(CreateAddressDTO createAddressDTO)
        {
            if (createAddressDTO.IsDefault)
            {
                var existingDefault = await _unitOfWork.Addresses.FindAsync(x => x.UserId == _userId && x.IsDefault);
                foreach (var adrs in existingDefault)
                {
                    adrs.IsDefault = false;
                }
            }

            var address = _mapper.Map<Address>(createAddressDTO);
            address.UserId = _userId;
            await _unitOfWork.Addresses.AddAsync(address);
            await _unitOfWork.SaveAsync();
            var addressDTO = _mapper.Map<AddressDTO>(address);
            return new ResponseModel<AddressDTO>
            {
                Data = addressDTO,
                IsSuccess = true,              
                Message = "Address created successfully."
            };
        }

        public async Task<ResponseModel<bool>> DeleteAddressAsync(Guid id)
        {
            var address = await _unitOfWork.Addresses.FindAsync(x => x.Id == id || x.UserId == _userId);
            if (address is null)
            {
                return new ResponseModel<bool>
                {
                    Data = false,
                    IsSuccess = false,
                    Message = "Address not found."
                };
            }

            await _unitOfWork.Addresses.SoftDeleteAsync(id);
            await _unitOfWork.SaveAsync();
            return new ResponseModel<bool>
            {
                Data = true,
                IsSuccess = true,
                Message = "Address deleted successfully."
            };
        }

        public async Task<ResponseModel<AddressDTO>> GetAddressByIdAsync(Guid id)
        {
            var addresses = _unitOfWork.Addresses.FindAsync(x => x.Id == id && x.UserId == _userId);
            if (addresses is null)
            {
                return new ResponseModel<AddressDTO>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = "Address not found."
                };
            }

            var addressDTO = _mapper.Map<AddressDTO>(addresses);
            return new ResponseModel<AddressDTO>
            {
                Data = addressDTO,
                IsSuccess = true,
                Message = "Address retrieved successfully."
            };
        }

        public async Task<ResponseModel<List<AddressDTO>>> GetAllAddressesAsync()
        {
            var addreses = _unitOfWork.Addresses.FindAsync(x => x.UserId == _userId).Result
                .OrderByDescending(x => x.IsDefault).ThenByDescending(x => x.CreatedAt).ToList();

            var addressDTOs = _mapper.Map<List<AddressDTO>>(addreses);
            return new ResponseModel<List<AddressDTO>>
            {
                Data = addressDTOs,
                IsSuccess = true,
                Message = "Addresses retrieved successfully."
            };
        }

        public async Task<ResponseModel<bool>> UpdateAddressAsync(Guid id, UpdateAddressDTO updateAddressDTO)
        {
            var address = _unitOfWork.Addresses.FindAsync(x => x.Id == id && x.UserId == _userId).Result;
            if (address is null)
            {
                return new ResponseModel<bool>
                {
                    IsSuccess = false,
                    Data = false,
                    Message = "Address not found."
                };
            }
            if (updateAddressDTO.IsDefault)
            {
                var existingDefault = _unitOfWork.Addresses.FindAsync(x => x.UserId == _userId && x.IsDefault).Result;
                foreach (var adrs in existingDefault)
                {
                    adrs.IsDefault = false;
                }
            }
            _mapper.Map(updateAddressDTO, address);
            var newAddress = address.FirstOrDefault();
            await _unitOfWork.Addresses.UpdateAsync(newAddress);
            _unitOfWork.SaveAsync().Wait();
            return new ResponseModel<bool>
            {
                IsSuccess = true,
                Data = true,
                Message = "Address updated successfully."
            };
        }
    }
}
