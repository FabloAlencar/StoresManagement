using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoresManagement.Data;
using StoresManagement.Models;
using StoresManagement.ViewModels;

namespace StoresManagement.Controllers
{
    [Authorize(Roles = "Manager,Administrator,Seller")]
    public class BranchesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly int _entityId;

        public BranchesController(ApplicationDbContext context, IMapper mapper, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
            _entityId = GetEntityId();
        }

        private int GetEntityId()
        {
            var entityUsers = _context.EntityUsers
        .SingleOrDefault(m => m.UserId == _userManager.GetUserId(_httpContextAccessor.HttpContext.User));

            if (entityUsers == null)
                return 0;
            else
                return entityUsers.EntityId;
        }

        // GET: Branches
        public async Task<IActionResult> Index()
        {
            var loggedUserId = _userManager.GetUserId(HttpContext.User);

            var branches = await _context.Branches
                .Where(m => m.EntityId == _entityId)
                .Include(b => b.Contact)
                .Include(b => b.Entity)
                .ToListAsync();

            return View(_mapper.Map<IEnumerable<BranchFormViewModel>>(branches));
        }

        // GET: Branches/ListBranches/5
        public async Task<IActionResult> ListBranches(int? entityId)
        {
            if (entityId == null && entityId != _entityId)
            {
                return NotFound();
            }

            var branches = await _context.Branches
                .Where(m => m.EntityId == entityId)
                .Include(b => b.Entity)
                .Include(b => b.Contact)
                .ToListAsync();

            return View(_mapper.Map<IEnumerable<BranchFormViewModel>>(branches));
        }

        // GET: Branches/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var branch = await _context.Branches
                .Include(b => b.Contact)
                .Include(b => b.Entity)
                .SingleOrDefaultAsync(m => m.EntityId == _entityId && m.Id == id);

            if (branch == null)
            {
                return NotFound();
            }

            return View(_mapper.Map<BranchFormViewModel>(branch));
        }

        // GET: Branches/Create
        [Authorize(Roles = "Manager, Administrator")]
        public IActionResult Create()
        {
            var branchVM = new BranchFormViewModel
            {
                Entities = _context.Entities.ToList()
            };

            return View(branchVM);
        }

        // POST: Branches/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager, Administrator")]
        public async Task<IActionResult> Create(BranchFormViewModel branchVM)
        {
            if (ModelState.IsValid)
            {
                var branch = _mapper.Map<Branch>(branchVM);

                _context.Add(branch);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            branchVM.Entities = _context.Entities.ToList();

            return View(branchVM);
        }

        // GET: Branches/Edit/5
        [Authorize(Roles = "Manager, Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var branch = await _context.Branches
                .Include(b => b.Contact)
                .Include(b => b.Entity)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (branch == null)
            {
                return NotFound();
            }

            var branchVM = _mapper.Map<BranchFormViewModel>(branch);

            branchVM.Entities = _context.Entities.ToList();

            return View(branchVM);
        }

        // POST: Branches/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager, Administrator")]
        public async Task<IActionResult> Edit(int id, BranchFormViewModel branchVM)
        {
            if (id != branchVM.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var branch = _mapper.Map<Branch>(branchVM);

                    _context.Update(branch);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await BranchExists(branchVM.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            branchVM.Entities = _context.Entities.ToList();

            return View(branchVM);
        }

        // GET: Branches/Delete/5
        [Authorize(Roles = "Manager, Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var branch = await _context.Branches
                .Include(b => b.Contact)
                .Include(b => b.Entity)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (branch == null)
            {
                return NotFound();
            }

            return View(_mapper.Map<BranchFormViewModel>(branch));
        }

        // POST: Branches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager, Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var branch = await _context.Branches.FindAsync(id);
            _context.Branches.Remove(branch);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> BranchExists(int id)
        {
            return await _context.Branches.AnyAsync(e => e.Id == id);
        }
    }
}