using Microsoft.AspNetCore.Identity;

namespace AroundMe.Models;

// Identity customization: extra profile fields in AspNetUsers.
public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = "";
    public string? City { get; set; }
    public int? LegacyUserId { get; set; }
}
