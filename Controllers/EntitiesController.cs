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
    public class EntitiesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public EntitiesController(ApplicationDbContext context, IMapper mapper, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Entities
        public async Task<IActionResult> Index()
        {
            var entities = await _context.Entities.ToListAsync();

            return View(_mapper.Map<IEnumerable<EntityFormViewModel>>(entities));
        }

        // GET: Entities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entity = await _context.Entities
                .SingleOrDefaultAsync(m => m.Id == id);
            if (entity == null)
            {
                return NotFound();
            }

            return View(_mapper.Map<EntityFormViewModel>(entity));
        }

        // GET: Entities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Entities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegisterFormViewModel registerFormVM)
        {
            if (ModelState.IsValid)
            {
                // Creating User
                var user = new IdentityUser
                {
                    UserName = registerFormVM.Email,
                    Email = registerFormVM.Email,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, registerFormVM.Password);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(registerFormVM);
                }
                await _context.SaveChangesAsync();

                // Adding role to the User
                var administratorRole = await _context.Roles
                .SingleOrDefaultAsync(m => m.Name == UserRoles.Administrator);

                var userRole = new IdentityUserRole<string>
                {
                    UserId = user.Id,
                    RoleId = administratorRole.Id
                };
                _context.Add(userRole);
                await _context.SaveChangesAsync();

                // Creating Entity
                var entity = _mapper.Map<Entity>(registerFormVM.Entity);
                _context.Add(entity);
                await _context.SaveChangesAsync();

                // Adding contact
                var contact = new Contact
                {
                    EntityId = entity.Id,
                    Email = registerFormVM.Email
                };
                _context.Add(contact);
                await _context.SaveChangesAsync();
                // Adding operator
                var sysOperator = new Operator
                {
                    EntityId = entity.Id,
                    UserId = user.Id,
                    RoleId = administratorRole.Id,
                    ContactId = contact.Id
                };
                _context.Add(sysOperator);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(registerFormVM);
        }

        // GET: Entities/Edit/5
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entity = await _context.Entities.FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            return View(_mapper.Map<EntityFormViewModel>(entity));
        }

        // POST: Entities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EntityFormViewModel entityVM)
        {
            if (id != entityVM.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var entity = _mapper.Map<Entity>(entityVM);

                    _context.Update(entity);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await EntityExists(entityVM.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(entityVM);
        }

        // GET: Entities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entity = await _context.Entities
                .SingleOrDefaultAsync(m => m.Id == id);
            if (entity == null)
            {
                return NotFound();
            }

            return View(_mapper.Map<EntityFormViewModel>(entity));
        }

        // POST: Entities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entity = await _context.Entities.FindAsync(id);

            entity.Active = false;
            _context.Entry(entity).Property("Active").IsModified = true;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> EntityExists(int id)
        {
            return await _context.Entities.AnyAsync(e => e.Id == id);
        }
    }
}