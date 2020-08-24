using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoresManagement.Constants;
using StoresManagement.Data;
using StoresManagement.Models;
using StoresManagement.ViewModels;

namespace StoresManagement.Controllers
{
    [Authorize(Roles = "Manager")]
    public class AccessesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<IdentityUser> _userManager;

        public AccessesController(ApplicationDbContext context, IMapper mapper, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        // GET: Administration
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users
                .ToListAsync();

            var rolesVM = new List<OperatorFormViewModel>();

            foreach (var user in users)
            {
                var accessVM = new OperatorFormViewModel();

                accessVM.User = user;

                var entityUser = await _context.Operators
                .Include(b => b.Entity)
                .FirstOrDefaultAsync(m => m.UserId == user.Id);

                if (entityUser != null)
                {
                    accessVM.Entity = entityUser.Entity;
                }

                var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(m => m.UserId == user.Id);

                if (userRole != null)
                {
                    accessVM.Role = await _context.Roles
                    .FirstOrDefaultAsync(m => m.Id == userRole.RoleId);
                }

                rolesVM.Add(accessVM);
            }

            return View(rolesVM);
        }

        public IActionResult Register()
        {
            var registerVM = new RegisterFormViewModel
            {
                Entities = _context.Entities.ToList()
            };

            return View(registerVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterFormViewModel registerVM)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    UserName = registerVM.UserName,
                    Email = registerVM.Email,
                    EmailConfirmed = true
                };

                // adicionar entides
                var result = await _userManager.CreateAsync(user, registerVM.Password);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                await _context.SaveChangesAsync();

                // Adding role to the User
                var sellerRole = await _context.Roles
                .SingleOrDefaultAsync(m => m.Name == UserRoles.Seller);

                var userRole = new IdentityUserRole<string>
                {
                    UserId = user.Id,
                    RoleId = sellerRole.Id
                };

                _context.Add(userRole);

                // Adding contact
                var contact = new Contact
                {
                    EntityId = registerVM.EntityId,
                    Email = registerVM.Email
                };
                _context.Add(contact);
                await _context.SaveChangesAsync();

                // Adding operator
                var sysOperator = new Operator
                {
                    EntityId = registerVM.EntityId,
                    UserId = user.Id,
                    RoleId = sellerRole.Id,
                    ContactId = contact.Id
                };
                _context.Add(sysOperator);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Accesses");
            }

            registerVM.Entities = _context.Entities.ToList();

            return View(registerVM);
        }

        // GET: Administration/Edit/5
        [Authorize(Roles = "Manager")]
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

            var operatorVM = new OperatorFormViewModel();

            operatorVM.User = user;

            var userRole = await _context.UserRoles
            .FirstOrDefaultAsync(m => m.UserId == user.Id);

            if (userRole != null)
            {
                operatorVM.Role = await _context.Roles
                .FirstOrDefaultAsync(m => m.Id == userRole.RoleId);
            }

            operatorVM.Roles = await _context.Roles
                 .Where(m => m.Name != "Manager")
                .ToListAsync();

            return View(operatorVM);
        }

        // POST: Administration/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Edit(string id, OperatorFormViewModel operatorVM)
        {
            if (id != operatorVM.User.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Updating the Username of the User
                    var user = await _context.Users
                    .SingleOrDefaultAsync(m => m.Id == operatorVM.User.Id);

                    if (user.UserName != operatorVM.User.UserName || user.PhoneNumber != operatorVM.User.PhoneNumber)
                    {
                        user.UserName = operatorVM.User.UserName;
                        user.PhoneNumber = operatorVM.User.PhoneNumber;
                        _context.Users.Update(user);
                    }

                    // Updating the Role of the User
                    var userRoleDB = await _context.UserRoles
                    .SingleOrDefaultAsync(m => m.UserId == operatorVM.User.Id);

                    if (userRoleDB == null || userRoleDB.RoleId != operatorVM.Role.Id)
                    {
                        if (userRoleDB != null)
                        {
                            _context.UserRoles.Remove(userRoleDB);
                        }

                        var userRole = new IdentityUserRole<string>
                        {
                            UserId = operatorVM.User.Id,
                            RoleId = operatorVM.Role.Id
                        };

                        _context.Add(userRole);
                    }

                    // Saving changes
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await UserExists(operatorVM.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(operatorVM);
        }

        private async Task<bool> UserExists(string id)
        {
            return await _context.Users.AnyAsync(e => e.Id == id);
        }
    }
}