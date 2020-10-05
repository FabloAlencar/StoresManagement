using Microsoft.AspNetCore.Identity;

namespace StoresManagement.Models
{
    public class Operator
    {
        public int EntityId { get; set; }

        public virtual Entity Entity { get; set; }

        public int Id { get; set; }

        public string UserId { get; set; }

        public virtual IdentityUser User { get; set; }

        public string RoleId { get; set; }

        public virtual IdentityRole Role { get; set; }

        public int ContactId { get; set; }

        public virtual Contact Contact { get; set; }
    }
}