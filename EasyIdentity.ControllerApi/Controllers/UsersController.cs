using EasyIdentity.Core.Constants;
using EasyIdentity.Core.Entities;
using EasyIdentity.Core.Services.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EasyIdentity.ControllerApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;

    public UsersController(UserManager<ApplicationUser> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    [Authorize]
    [HttpGet]
    public IActionResult Get()
        => Ok("Ok");

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        var user = await _userManager.FindByNameAsync(loginDto.UserName!);
        if (user is null)
            return Unauthorized();

        var result = await _userManager.CheckPasswordAsync(user, loginDto.Password!);
        if (result)
        {
            var token = _tokenService.CreateToken(user);
            // Append the token to Cookies
            HttpContext.Response.Cookies.Append(AuthenticationConstants.CookieAccessToken, token);
            return Ok(new
            {
                user.UserName,
                user.FullName,
                Token = token,
            });
        }

        return Unauthorized();
    }

    public record LoginDto(string? UserName, string? Password);
}
