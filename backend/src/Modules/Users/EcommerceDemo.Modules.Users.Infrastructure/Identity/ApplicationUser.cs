using Microsoft.AspNetCore.Identity;

namespace EcommerceDemo.Modules.Users.Infrastructure.Identity;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public string DisplayName { get; set; } = string.Empty;
}
