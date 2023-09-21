using Microsoft.AspNetCore.Identity;

namespace EasyIdentity.Core.Services;

public class UserService : IUserService
{
    private readonly SignInManager<IdentityUser> _signInManager;

    public UserService(SignInManager<IdentityUser> signInManager)
    {
        _signInManager = signInManager;
    }
    public async Task<SignInResult> Login(string userName, string password, bool rememberMe = false)
    {
        var user = await _signInManager.UserManager
            .FindByNameAsync(userName);

        if (user == null)
            return SignInResult.Failed;

        var result = await _signInManager.PasswordSignInAsync(user, password, rememberMe, false);

        return result;
    }

    public Task Logout()
    {
        return _signInManager.SignOutAsync();
    }

    public async Task<IdentityResult> Register(string userName, string email, string password)
    {
        var user = new IdentityUser
        {
            UserName = userName,
            Email = email
        };
        var result = await _signInManager.UserManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, isPersistent: false);
        }
        return result;
    }
}
