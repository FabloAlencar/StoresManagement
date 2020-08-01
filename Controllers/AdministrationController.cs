using System.Collections.Generic;
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
            var userRoles = await _context.UserRoles.ToListAsync();

            var userRolesVM = new List<RoleFormViewModel>();

            foreach (var userRole in userRoles)
            {
                var userRoleVM = new RoleFormViewModel();

                userRoleVM.User = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == userRole.UserId);

                userRoleVM.Role = await _context.Roles
                .FirstOrDefaultAsync(m => m.Id == userRole.RoleId);

                userRolesVM.Add(userRoleVM);
            }

            return View(userRolesVM);
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

            return View(_mapper.Map<RoleFormViewModel>(role));
        }

        // GET: Administration/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Administration/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleFormViewModel roleVM)
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
    }
}