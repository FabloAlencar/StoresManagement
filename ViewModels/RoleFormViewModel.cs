using Microsoft.AspNetCore.Identity;

namespace StoresManagement.ViewModels
{
    public class RoleFormViewModel
    {
        public virtual IdentityUser User { get; set; }

        public virtual IdentityRole Role { get; set; }
    }
}