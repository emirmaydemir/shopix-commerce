using shopix_core_domain.Entities;

namespace shopix_commerce_infrastructure.Extensions.TokenExtensions
{
    public interface ITokenService
    {
        string CreateToken(ApplicationUser user, IList<string> roles);
    }
}
