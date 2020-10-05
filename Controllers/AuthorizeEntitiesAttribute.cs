using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using StoresManagement.Constants;
using StoresManagement.Data;
using System.Collections.Generic;
using System.Linq;

namespace StoresManagement.Controllers
{
    public static class AuthorizeEntitiesAttribute
    {
        public static List<int> GetEntityIds
        (ApplicationDbContext _context,
        UserManager<IdentityUser> _userManager,
        IHttpContextAccessor _httpContextAccessor)

        {
            var entityIds = new List<int>();

            var entityUser = _context.Operators.SingleOrDefault(m => m.UserId == _userManager.GetUserId(_httpContextAccessor.HttpContext.User));

            if (entityUser != null)
            {
                entityIds.Add(entityUser.EntityId);
            }
            else
            {
                var userRole = (from t1 in _context.UserRoles
                                from t2 in _context.Roles
                                             .Where(o => t1.RoleId == o.Id && t1.UserId == _userManager.GetUserId(_httpContextAccessor.HttpContext.User))
                                select t2.Name).First();

                if (userRole == UserRoles.Manager)
                {
                    entityIds = _context.Operators.Select(m => m.EntityId).ToList();
                }
            }

            return entityIds;
        }
    }
}