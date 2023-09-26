using EasyIdentity.Core.Entities;

namespace EasyIdentity.Core.Services.Authentication;
public interface ITokenService
{
    string CreateToken(ApplicationUser user);
}