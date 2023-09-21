using Microsoft.AspNetCore.Identity;

namespace EasyIdentity.Core.Services;

public class UserService : IUserService
{
    private readonly SignInManager<IdentityUser> _signInManager;

    public UserService(SignInManager<IdentityUser> signInManager)
    {
        _signInManager = signInManager;
    }
    public async Task<SignInResult> Login(string userName, string password)
    {
        var user = await _signInManager.UserManager
            .FindByNameAsync(userName);

        if (user == null)
            return SignInResult.Failed;

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);

        return result;
    }

    public async Task<IdentityResult> Register(string userName, string email, string password)
    {
        var user = new IdentityUser
        {
            UserName = userName,
            Email = email
        };
        var result = await _signInManager.UserManager.CreateAsync(user, password);
        return result;
    }
}
