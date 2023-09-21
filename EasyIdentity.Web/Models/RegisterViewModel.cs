namespace EasyIdentity.Web.Models;

public class RegisterViewModel
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string ReturnUrl { get; set; } = string.Empty;
}