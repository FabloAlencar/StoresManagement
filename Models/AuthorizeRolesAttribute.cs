using Microsoft.AspNetCore.Authorization;
using System;

namespace StoresManagement.Models
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        public AuthorizeRolesAttribute(params string[] roles)
        {
            Roles = String.Join(",", roles);
        }
    }
}