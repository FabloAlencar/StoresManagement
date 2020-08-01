using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace StoresManagement.ViewModels
{
    public class UserRoleFormViewModel
    {
        public UserRoleFormViewModel()
        {
            Roles = new List<IdentityRole>();
        }

        public IEnumerable<IdentityRole> Roles { get; set; }

        public virtual IdentityUser User { get; set; }

        public virtual IdentityRole Role { get; set; }
    }
}