using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace shopix_commerce_infrastructure.CurrentUser
{
    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);

        public string? FirstName => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.GivenName);

        public string? LastName => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Surname);

        public string FullName => $"{FirstName} {LastName}".Trim();

        public IList<string> Roles
        {
            get
            {
                var claims = _httpContextAccessor.HttpContext?.User?.FindAll(ClaimTypes.Role);
                return claims?.Select(c => c.Value).ToList() ?? new List<string>();
            }
        }

        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}
