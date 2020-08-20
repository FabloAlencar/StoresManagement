using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoresManagement.Constants;
using StoresManagement.Data;
using StoresManagement.Models;
using StoresManagement.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace StoresManagement.Controllers
{
    [Authorize(Roles = "Manager")]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(ApplicationDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
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
                    Email = registerVM.Email
                };

                // adicionar entides
                var result = await _userManager.CreateAsync(user, registerVM.Password);
                if (result.Succeeded)
                {
                    await _context.SaveChangesAsync();

                    // Creating Enity & User relationship
                    var entityUser = new EntityUser
                    {
                        EntityId = registerVM.EntityId,
                        UserId = user.Id
                    };
                    _context.Add(entityUser);

                    // Adding role to the User
                    var administratorRole = await _context.Roles
                    .SingleOrDefaultAsync(m => m.Name == UserRoles.Seller);

                    if (administratorRole != null)
                    {
                        var userRole = new IdentityUserRole<string>
                        {
                            UserId = user.Id,
                            RoleId = administratorRole.Id
                        };

                        _context.Add(userRole);
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Accesses");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            registerVM.Entities = _context.Entities.ToList();

            return View(registerVM);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(loginVM.Email, loginVM.Password, loginVM.RememberMe, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            }
            return View(loginVM);
        }
    }
}