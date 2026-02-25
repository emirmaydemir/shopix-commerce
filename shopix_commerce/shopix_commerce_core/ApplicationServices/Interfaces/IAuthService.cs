using shopix_commerce_core.DTO;
using shopix_commerce_infrastructure.Models;

namespace shopix_commerce_core.ApplicationServices.Interfaces
{
    public interface IAuthService
    {
        Task<ResponseModel<UserDTO>> Register(RegisterDTO dto);
        Task<ResponseModel<UserDTO>> Login(LoginDTO dto);
    }
}
