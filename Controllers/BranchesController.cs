using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoresManagement.Constants;
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
        private readonly List<int> _entityIds = new List<int>();

        public BranchesController(ApplicationDbContext context, IMapper mapper, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
            _entityIds = GetEntityId();
        }

        private List<int> GetEntityId()
        {
            var entityIds = new List<int>();

            var entityUser = _context.Operators
                .Include(b => b.Role)
                .Include(b => b.Entity)
                .SingleOrDefault(m => m.UserId == _userManager.GetUserId(_httpContextAccessor.HttpContext.User));

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
                    var entityUsers = _context.Operators.Select(m => new { m.EntityId }).ToList();

                    foreach (var user in entityUsers)
                    {
                        entityIds.Add(user.EntityId);
                    }
                }
            }

            return entityIds;
        }

        // GET: Branches/ListAll
        [HttpGet]
        public ActionResult ListAll()
        {
            var list = _context.Branches
                .Where(b => _entityIds.Contains(b.EntityId))
                .Select(r => new
                {
                    id = r.Id,
                    entity = r.Entity.Name,
                    identification = r.Identification,
                    name = r.Name,
                    address = r.Contact.Address,
                    active = r.Active
                }).ToArray();

            var dataPage = new
            {
                last_page = 0,
                data = list
            };

            return Json(dataPage);
        }

        // GET: Branches
        public IActionResult Index()
        {
            return View();
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
                .SingleOrDefaultAsync(m => _entityIds.Contains(m.EntityId) && m.Id == id);

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
                Entities = _context.Entities.Where(b => _entityIds.Contains(b.Id)).ToList()
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
                branch.Contact.EntityId = branch.EntityId;

                _context.Add(branch);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            branchVM.Entities = _context.Entities.Where(b => _entityIds.Contains(b.Id)).ToList();

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

            branchVM.Entities = _context.Entities.Where(b => _entityIds.Contains(b.Id)).ToList();

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
            branchVM.Entities = _context.Entities.Where(b => _entityIds.Contains(b.Id)).ToList();

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

            branch.Active = false;
            _context.Entry(branch).Property("Active").IsModified = true;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> BranchExists(int id)
        {
            return await _context.Branches.AnyAsync(e => e.Id == id);
        }
    }
}