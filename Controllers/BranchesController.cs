using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoresManagement.Data;
using StoresManagement.Models;
using StoresManagement.ViewModels;

namespace StoresManagement.Controllers
{
    public class BranchesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public BranchesController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: Branches
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Branches.Include(b => b.Contact).Include(b => b.Entity);

            var branches = await applicationDbContext.ToListAsync();
            var branchesVM = new List<BranchFormViewModel>();

            foreach (var branch in branches)
            {
                var branchVM = new BranchFormViewModel();

                branchVM = _mapper.Map<BranchFormViewModel>(branch);

                branchesVM.Add(branchVM);
            }

            return View(branchesVM);
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
                .FirstOrDefaultAsync(m => m.Id == id);
            if (branch == null)
            {
                return NotFound();
            }

            var branchVM = _mapper.Map<BranchFormViewModel>(branch);

            return View(branchVM);
        }

        // GET: Branches/Create
        public IActionResult Create()
        {
            ViewData[nameof(Branch.EntityId)] = new SelectList(_context.Entities, nameof(Entity.Id), nameof(Entity.Name));
            return View();
        }

        // POST: Branches/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BranchFormViewModel branchVM)
        {
            if (ModelState.IsValid)
            {
                var branch = _mapper.Map<Branch>(branchVM);

                _context.Add(branch);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData[nameof(Branch.EntityId)] = new SelectList(_context.Entities, nameof(Entity.Id), nameof(Entity.Name));

            return View(branchVM);
        }

        // GET: Branches/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var branch = await _context.Branches
                .Include(b => b.Contact)
                .Include(b => b.Entity)
                .FirstOrDefaultAsync(m => m.Id == id);

            //var branch = await _context.Branches.FindAsync(id);

            if (branch == null)
            {
                return NotFound();
            }
            ViewData[nameof(Branch.EntityId)] = new SelectList(_context.Entities, nameof(Entity.Id), nameof(Entity.Name));

            var branchVM = _mapper.Map<BranchFormViewModel>(branch);

            return View(branchVM);
        }

        // POST: Branches/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BranchFormViewModel branchVM)
        {
            if (id != branchVM.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var branch = _mapper.Map<Branch>(branchVM);
                    _context.Update(branch);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BranchExists(branchVM.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData[nameof(Branch.EntityId)] = new SelectList(_context.Entities, nameof(Entity.Id), nameof(Entity.Name));

            return View(branchVM);
        }

        // GET: Branches/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var branch = await _context.Branches
                .Include(b => b.Contact)
                .Include(b => b.Entity)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (branch == null)
            {
                return NotFound();
            }

            var branchVM = _mapper.Map<BranchFormViewModel>(branch);

            return View(branchVM);
        }

        // POST: Branches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var branch = await _context.Branches.FindAsync(id);
            _context.Branches.Remove(branch);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BranchExists(int id)
        {
            return _context.Branches.Any(e => e.Id == id);
        }
    }
}