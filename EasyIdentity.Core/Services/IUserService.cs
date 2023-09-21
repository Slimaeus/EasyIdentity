using Microsoft.AspNetCore.Identity;

namespace EasyIdentity.Core.Services;
public interface IUserService
{
    Task<IdentityResult> Login(string userName, string password);
    Task<IdentityResult> Register(string userName, string email, string password);
}
