#nullable disable
using Microsoft.AspNetCore.Identity;

namespace MrpSystem.Server.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}
