using Microsoft.AspNetCore.Identity;

namespace AroundMe.Models;
public class ApplicationUser : IdentityUser
{
    public string FullName
    {
        get;
        set;
    }
    = "";
    public int? LegacyUserId
    {
        get;
        set;
    }
}
