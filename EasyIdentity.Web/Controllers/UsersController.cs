using EasyIdentity.Core.Services;
using EasyIdentity.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace EasyIdentity.Web.Controllers;
public class UsersController : Controller
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Login(string? returnUrl)
    {
        return View(new LoginViewModel { ReturnUrl = returnUrl ?? string.Empty });
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel loginViewModel)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }
        var result = await _userService.Login(loginViewModel.UserName, loginViewModel.Password, loginViewModel.RememberMe);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View();
        }
        if (!string.IsNullOrEmpty(loginViewModel.ReturnUrl))
        {
            return LocalRedirect(loginViewModel.ReturnUrl);
        }
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Logout(string returnUrl)
    {
        await _userService.Logout();
        if (returnUrl != null)
        {
            return LocalRedirect(returnUrl);
        }
        else
        {
            return RedirectToAction("Index", "Home");
        }
    }
    public IActionResult Register(string? returnUrl)
    {
        return View(new RegisterViewModel { ReturnUrl = returnUrl ?? string.Empty });
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }
        var result = await _userService.Register(registerViewModel.UserName, registerViewModel.Email, registerViewModel.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View();
        }
        if (!string.IsNullOrEmpty(registerViewModel.ReturnUrl))
        {
            return LocalRedirect(registerViewModel.ReturnUrl);
        }
        return RedirectToAction("Index", "Home");
    }
}
