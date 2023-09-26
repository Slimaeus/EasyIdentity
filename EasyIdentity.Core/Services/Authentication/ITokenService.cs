using Microsoft.AspNetCore.Identity;

namespace EasyIdentity.Core.Services.Authentication;
public interface ITokenService
{
    string CreateToken(IdentityUser user);
}