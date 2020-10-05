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
    [Authorize(Policy = "Manager")]
    public class AccessesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<IdentityUser> _userManager;

        public AccessesController(ApplicationDbContext context,
            IMapper mapper,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        // GET: Customers/ListAll
        [HttpGet]
        public ActionResult ListAll()
        {
            var list = _context.Operators
                .Select(r => new
                {
                    id = r.Id,
                    entity = r.Entity.Name,
                    userName = r.User.UserName,
                    role = r.Role.Name,
                    email = r.User.Email,
                    phonenumber = r.User.PhoneNumber
                }).ToArray();

            return Json(new
            {
                last_page = 0,
                data = list
            });
        }

        // GET: Administration
        public IActionResult Index()
        {
            return View();
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
            if (!ModelState.IsValid)
            {
                registerVM.Entities = _context.Entities.ToList();

                return View(registerVM);
            }
            var user = new IdentityUser
            {
                UserName = registerVM.Email,
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

            return RedirectToAction(nameof(Index));
        }

        // GET: Administration/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sysOperator = await _context.Operators
                .Include(b => b.Entity)
                .Include(b => b.User)
                .Include(b => b.Role)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (sysOperator == null)
            {
                return NotFound();
            }

            var operatorVM = _mapper.Map<OperatorFormViewModel>(sysOperator);

            operatorVM.Roles = await _context.Roles
                 .Where(m => m.Name != UserRoles.Manager)
                .ToListAsync();

            return View(operatorVM);
        }

        // POST: Administration/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OperatorFormViewModel operatorVM)
        {
            if (id != operatorVM.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Updating the Username of the User
                    var sysOperator = await _context.Operators
                    .Include(b => b.User)
                    .Include(b => b.Role)
                    .Include(b => b.Contact)
                    .SingleOrDefaultAsync(m => m.Id == operatorVM.Id);

                    if (sysOperator == null)
                    {
                        return NotFound();
                    }

                    if (sysOperator.User.UserName != operatorVM.User.UserName || sysOperator.User.PhoneNumber != operatorVM.User.PhoneNumber)
                    {
                        sysOperator.User.UserName = operatorVM.User.UserName;
                        sysOperator.User.PhoneNumber = sysOperator.Contact.PhoneNumber = operatorVM.User.PhoneNumber;
                        _context.Users.Update(sysOperator.User);
                        _context.Contacts.Update(sysOperator.Contact);
                    }

                    // Updating the Role of the User
                    var userRoleDB = await _context.UserRoles
                    .SingleOrDefaultAsync(m => m.UserId == sysOperator.UserId);

                    if (userRoleDB == null)
                    {
                        return NotFound();
                    }

                    if (sysOperator.RoleId != operatorVM.Role.Id)
                    {
                        _context.UserRoles.Remove(userRoleDB);

                        var userRole = new IdentityUserRole<string>
                        {
                            UserId = operatorVM.UserId,
                            RoleId = sysOperator.RoleId = operatorVM.Role.Id
                        };

                        _context.Add(userRole);
                        _context.Operators.Update(sysOperator);
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