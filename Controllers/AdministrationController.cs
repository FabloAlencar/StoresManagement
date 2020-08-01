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
            var users = await _context.Users.Where(m => m.Email != "admin@sm.com").ToListAsync();

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

        // GET: Administration/Details/5
        public async Task<IActionResult> Details(string roleName)
        {
            if (roleName == null)
            {
                return NotFound();
            }

            var role = await _context.Roles
                .SingleOrDefaultAsync(m => m.Name == roleName);
            if (role == null)
            {
                return NotFound();
            }

            return View(_mapper.Map<UserRoleFormViewModel>(role));
        }

        // GET: Administration/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Administration/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserRoleFormViewModel roleVM)
        {
            if (ModelState.IsValid)
            {
                var role = _mapper.Map<IdentityRole>(roleVM);

                _context.Add(role);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(roleVM);
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

            var userRole = await _context.UserRoles
            .FirstOrDefaultAsync(m => m.UserId == user.Id);

            userRoleVM.User = user;

            if (userRole != null)
            {
                userRoleVM.Role = await _context.Roles
                .FirstOrDefaultAsync(m => m.Id == userRole.RoleId);
            }

            // userRoleVM.Roles = _context.Roles.ToList();
            userRoleVM.Roles = await _context.Roles.Where(m => m.Name != "Manager").ToListAsync();

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
                    var userRole = await _context.UserRoles
                    .SingleOrDefaultAsync(m => m.UserId == userRoleVM.User.Id);

                    if (userRole != null)
                    {
                        _context.UserRoles.Remove(userRole);
                    }

                    userRole = new IdentityUserRole<string>();

                    userRole.UserId = userRoleVM.User.Id;
                    userRole.RoleId = userRoleVM.Role.Id;

                    _context.Add(userRole);
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

        //// GET: Administration/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var role = await _context.Roles
        //        .SingleOrDefaultAsync(m => m.Id == id);
        //    if (role == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(_mapper.Map<RoleFormViewModel>(role));
        //}

        //// POST: Administration/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var role = await _context.Roles.FindAsync(id);
        //    _context.Roles.Remove(role);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private async Task<bool> RoleExists(int id)
        //{
        //    return await _context.Roles.AnyAsync(e => e.Id == id);
        //}

        private async Task<bool> UserExists(string id)
        {
            return await _context.Users.AnyAsync(e => e.Id == id);
        }
    }
}