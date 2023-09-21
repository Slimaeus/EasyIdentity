namespace EasyIdentity.Web.Models;

public class LoginViewModel
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; } = false;
    public string ReturnUrl { get; set; } = string.Empty;
}