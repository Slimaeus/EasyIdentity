using EasyIdentity.Core.Services;
using EasyIdentity.Web.Models.Api.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyIdentity.Web.Controllers.Api;

[Route("api/Users")]
[ApiController]
[Tags("Users")]
public class ApiUsersController : ControllerBase
{
    private readonly IUserService _userService;

    public ApiUsersController(IUserService userService)
        => _userService = userService;

    [HttpPost("authorize")]
    public async Task<ActionResult<string>> Authorize(AuthorizeDto authorizeDto)
    {
        var result = await _userService.Authorize(authorizeDto.UserName, authorizeDto.Password);
        if (string.IsNullOrEmpty(result))
        {
            return Unauthorized();
        }
        return Ok(result);
    }

    [Authorize]
    [HttpGet("check-authorize")]
    public ActionResult<string> CheckAuthorize()
    {
        return Ok("Ok");
    }
}
