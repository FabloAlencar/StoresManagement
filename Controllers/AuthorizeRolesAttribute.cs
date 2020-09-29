using Microsoft.AspNetCore.Authorization;
using System;

namespace StoresManagement.Controllers
{
    public sealed class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        public AuthorizeRolesAttribute(params string[] roles)
        {
            Roles = String.Join(",", roles);
        }
    }
}