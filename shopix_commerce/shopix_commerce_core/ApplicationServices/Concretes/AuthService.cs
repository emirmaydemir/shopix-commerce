using Microsoft.AspNetCore.Identity;
using shopix_commerce_core.ApplicationServices.Interfaces;
using shopix_commerce_core.DTO;
using shopix_commerce_infrastructure.Extensions.TokenExtensions;
using shopix_commerce_infrastructure.Models;
using shopix_core_domain.Entities;

namespace shopix_commerce_core.ApplicationServices.Concretes
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly ResponseModel<UserDTO> _responseModel;

        public AuthService(ResponseModel<UserDTO> responseModel, ITokenService tokenService, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _responseModel = responseModel;
        }
        public async Task<ResponseModel<UserDTO>> Login(LoginDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                _responseModel.IsSuccess = false;
                _responseModel.Message = "Invalid email or password.";
                return _responseModel;
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!result.Succeeded)
            {
                _responseModel.IsSuccess = false;
                _responseModel.Message = "Invalid email or password.";
                return _responseModel;
            }

            var roles = await _userManager.GetRolesAsync(user);
            _responseModel.IsSuccess = true;
            _responseModel.Data = new UserDTO
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = _tokenService.CreateToken(user, roles)
            };
            return _responseModel;
        }

        public async Task<ResponseModel<UserDTO>> Register(RegisterDTO dto)
        {
            var user = new ApplicationUser
            {
                Email = dto.Email,
                UserName = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                _responseModel.IsSuccess = false;
                string messages = "";
                foreach (var item in result.Errors)
                {
                    messages = messages + item.Description;
                }
                _responseModel.Message = messages;
                return _responseModel;
            }

            await _userManager.AddToRoleAsync(user, "User");
            var roles = await _userManager.GetRolesAsync(user);
            _responseModel.IsSuccess = true;
            _responseModel.Data = new UserDTO
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = _tokenService.CreateToken(user, roles)
            };
            return _responseModel;
        }
    }
}
