using System.Collections.Generic;
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
        public async Task<IActionResult> Create(EntityFormViewModel entityVM)
        {
            if (ModelState.IsValid)
            {
                // Creating User
                var user = new IdentityUser
                {
                    UserName = entityVM.Email,
                    Email = entityVM.Email
                };

                var result = await _userManager.CreateAsync(user, entityVM.Password);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(entityVM);
                }

                // Creating Entity
                var entity = _mapper.Map<Entity>(entityVM);

                _context.Add(entity);
                await _context.SaveChangesAsync();

                // Creating Enity & User relationship
                var entityUser = new EntityUser
                {
                    EntityId = entity.Id,
                    UserId = user.Id
                };
                _context.Add(entityUser);

                // Adding role to the User
                var administratorRole = await _context.Roles
                .SingleOrDefaultAsync(m => m.Name == UserRoles.Administrator);

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
                return RedirectToAction(nameof(Index));
            }
            return View(entityVM);
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
            var entityUser = await _context.EntityUsers.SingleOrDefaultAsync(m => m.EntityId == id);
            var user = await _context.Users.SingleOrDefaultAsync(m => m.Id == entityUser.UserId);
            var userRole = await _context.UserRoles.SingleOrDefaultAsync(m => m.UserId == entityUser.UserId);
            _context.Entities.Remove(entity);
            _context.EntityUsers.Remove(entityUser);
            _context.Users.Remove(user);
            _context.UserRoles.Remove(userRole);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> EntityExists(int id)
        {
            return await _context.Entities.AnyAsync(e => e.Id == id);
        }
    }
}