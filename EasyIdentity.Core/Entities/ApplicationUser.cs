using Microsoft.AspNetCore.Identity;

namespace EasyIdentity.Core.Entities;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}
