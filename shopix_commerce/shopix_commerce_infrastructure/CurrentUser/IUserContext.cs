using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shopix_commerce_infrastructure.CurrentUser
{
    public interface IUserContext
    {
        string? UserId { get; }
        string? Email { get; }
        string? FirstName { get; }
        string? LastName { get; }
        string FullName { get; }
        IList<string> Roles { get; }
        bool IsAuthenticated { get; }
    }
}
