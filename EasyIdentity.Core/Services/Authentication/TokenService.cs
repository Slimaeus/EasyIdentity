using EasyIdentity.Core.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EasyIdentity.Core.Services.Authentication;

public class TokenService : ITokenService
{
    private readonly string _tokenKey;
    private readonly TimeSpan _tokenLifespan;
    private readonly SigningCredentials _signingCredentials;

    public TokenService(IConfiguration configuration)
    {
        _tokenKey = configuration["TokenKey"] ?? throw new InvalidOperationException("The Token Key not found!");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenKey));
        _signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
        _tokenLifespan = TimeSpan.FromHours(5);
    }

    public string CreateToken(ApplicationUser user)
    {
        var tokenClaims = new List<Claim>
        {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.GivenName, user.FullName),
                new Claim(ClaimTypes.Email, user.Email!)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(tokenClaims),
            Expires = DateTime.Now.Add(_tokenLifespan),
            SigningCredentials = _signingCredentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public static DateTime GetExpireDate(string token)
    {
        JwtSecurityToken jwtToken = new(token);
        if (token == null)
            return DateTime.Now;
        return jwtToken.ValidTo.ToUniversalTime();
    }
}
