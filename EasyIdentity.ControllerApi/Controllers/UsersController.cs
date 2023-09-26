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
    {
        return Ok("Ok");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        var user = await _userManager.FindByNameAsync(loginDto.UserName!);
        if (user == null)
            return Unauthorized();

        var result = await _userManager.CheckPasswordAsync(user, loginDto.Password!);
        if (result)
        {
            var token = _tokenService.CreateToken(user);
            return Ok(new
            {
                UserName = user.UserName,
                FullName = user.FullName,
                Token = token,
            });
        }
        return Unauthorized();
    }

    public class LoginDto
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
    }
}
