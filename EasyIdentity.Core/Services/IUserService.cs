using Microsoft.AspNetCore.Identity;

namespace EasyIdentity.Core.Services;
public interface IUserService
{
    Task<SignInResult> Login(string userName, string password, bool rememberMe);
    Task<IdentityResult> Register(string userName, string email, string password);
    Task Logout();
}
