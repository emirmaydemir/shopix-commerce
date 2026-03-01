using shopix_commerce_core.DTO.Address;
using shopix_commerce_infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shopix_commerce_core.ApplicationServices.Interfaces
{
    public interface IAddressService
    {
        Task<ResponseModel<AddressDTO>> CreateAddressAsync(CreateAddressDTO createAddressDTO);
        Task<ResponseModel<bool>> UpdateAddressAsync(Guid id, UpdateAddressDTO updateAddressDTO);
        Task<ResponseModel<bool>> DeleteAddressAsync(Guid id);
        Task<ResponseModel<AddressDTO>> GetAddressByIdAsync(Guid id);
        Task<ResponseModel<List<AddressDTO>>> GetAllAddressesAsync();
    }
}
