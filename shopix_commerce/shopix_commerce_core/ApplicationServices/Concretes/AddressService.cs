using AutoMapper;
using Microsoft.AspNetCore.Http;
using shopix_commerce_core.ApplicationServices.Interfaces;
using shopix_commerce_core.DTO.Address;
using shopix_commerce_infrastructure.CurrentUser;
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
        private readonly IUserContext _userContext;
        public AddressService(IMapper mapper, IUnitOfWork unitOfWork, IUserContext userContext)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userContext = userContext;
        }
        public async Task<ResponseModel<AddressDTO>> CreateAddressAsync(CreateAddressDTO createAddressDTO)
        {
            if (createAddressDTO.IsDefault)
            {
                var existingDefault = await _unitOfWork.Addresses.FindAsync(x => x.UserId == _userContext.UserId && x.IsDefault);
                foreach (var adrs in existingDefault)
                {
                    adrs.IsDefault = false;
                }
            }

            var address = _mapper.Map<Address>(createAddressDTO);
            address.UserId = _userContext.UserId;
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
            var address = await _unitOfWork.Addresses.FindAsync(x => x.Id == id || x.UserId == _userContext.UserId);
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
            var addresses = await _unitOfWork.Addresses.FindAsync(x => x.Id == id && x.UserId == _userContext.UserId);
            if (addresses is null || !addresses.Any())
            {
                return new ResponseModel<AddressDTO>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = "Address not found."
                };
            }

            var address = addresses.FirstOrDefault()!;
            var addressDTO = _mapper.Map<AddressDTO>(address);

            return new ResponseModel<AddressDTO>
            {
                Data = addressDTO,
                IsSuccess = true,
                Message = "Address retrieved successfully."
            };
        }

        public async Task<ResponseModel<List<AddressDTO>>> GetAllAddressesAsync()
        {
            var addresses = await _unitOfWork.Addresses.FindAsync(x => x.UserId == _userContext.UserId);
            var userAddresses = addresses
                .OrderByDescending(x => x.IsDefault)
                .ThenByDescending(x => x.CreatedAt)
                .ToList();

            var addressDTOs = _mapper.Map<List<AddressDTO>>(userAddresses);

            return new ResponseModel<List<AddressDTO>>
            {
                Data = addressDTOs,
                IsSuccess = true,
                Message = "Addresses retrieved successfully."
            };
        }

        public async Task<ResponseModel<bool>> UpdateAddressAsync(Guid id, UpdateAddressDTO updateAddressDTO)
        {
            var addresses = await _unitOfWork.Addresses.FindAsync(x => x.Id == id && x.UserId == _userContext.UserId);
            if (addresses is null || !addresses.Any())
            {
                return new ResponseModel<bool>
                {
                    IsSuccess = false,
                    Data = false,
                    Message = "Address not found."
                };
            }

            var address = addresses.FirstOrDefault()!;
            if (updateAddressDTO.IsDefault)
            {
                var existingDefaults = await _unitOfWork.Addresses.FindAsync(
                    x => x.UserId == _userContext.UserId && x.IsDefault && x.Id != id);

                foreach (var existingDefault in existingDefaults)
                {
                    existingDefault.IsDefault = false;
                    await _unitOfWork.Addresses.UpdateAsync(existingDefault);
                }
            }

            _mapper.Map(updateAddressDTO, address);

            await _unitOfWork.Addresses.UpdateAsync(address);
            await _unitOfWork.SaveAsync();

            return new ResponseModel<bool>
            {
                IsSuccess = true,
                Data = true,
                Message = "Address updated successfully."
            };
        }
    }
}
