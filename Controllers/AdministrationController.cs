using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoresManagement.Data;
using StoresManagement.ViewModels;

namespace StoresManagement.Controllers
{
    public class AdministrationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AdministrationController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: Administration
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users
                .ToListAsync();

            var rolesVM = new List<UserRoleFormViewModel>();

            foreach (var user in users)
            {
                var userRoleVM = new UserRoleFormViewModel();

                var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(m => m.UserId == user.Id);

                userRoleVM.User = user;

                if (userRole != null)
                {
                    userRoleVM.Role = await _context.Roles
                    .FirstOrDefaultAsync(m => m.Id == userRole.RoleId);
                }

                rolesVM.Add(userRoleVM);
            }

            return View(rolesVM);
        }

        // GET: Administration/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var userRoleVM = new UserRoleFormViewModel();

            userRoleVM.User = user;

            var userRole = await _context.UserRoles
            .FirstOrDefaultAsync(m => m.UserId == user.Id);

            if (userRole != null)
            {
                userRoleVM.Role = await _context.Roles
                .FirstOrDefaultAsync(m => m.Id == userRole.RoleId);
            }

            userRoleVM.Roles = await _context.Roles
                 .Where(m => m.Name != "Manager")
                .ToListAsync();

            return View(userRoleVM);
        }

        // POST: Administration/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UserRoleFormViewModel userRoleVM)
        {
            if (id != userRoleVM.User.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Updating the Username of the User
                    var user = await _context.Users
                    .SingleOrDefaultAsync(m => m.Id == userRoleVM.User.Id);

                    if (user.UserName != userRoleVM.User.UserName || user.PhoneNumber != userRoleVM.User.PhoneNumber)
                    {
                        user.UserName = userRoleVM.User.UserName;
                        user.PhoneNumber = userRoleVM.User.PhoneNumber;
                        _context.Users.Update(user);
                    }

                    // Updating the Role of the User

                    var userRoleDB = await _context.UserRoles
                    .SingleOrDefaultAsync(m => m.UserId == userRoleVM.User.Id);

                    if (userRoleDB == null || userRoleDB.RoleId != userRoleVM.Role.Id)
                    {
                        if (userRoleDB != null)
                        {
                            _context.UserRoles.Remove(userRoleDB);
                        }

                        var userRole = new IdentityUserRole<string>
                        {
                            UserId = userRoleVM.User.Id,
                            RoleId = userRoleVM.Role.Id
                        };

                        _context.Add(userRole);
                    }

                    // Saving changes
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await UserExists(userRoleVM.User.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(userRoleVM);
        }

        private async Task<bool> UserExists(string id)
        {
            return await _context.Users.AnyAsync(e => e.Id == id);
        }
    }
}