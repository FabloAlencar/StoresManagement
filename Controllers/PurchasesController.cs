using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoresManagement.Data;
using StoresManagement.Models;
using StoresManagement.ViewModels;

namespace StoresManagement.Controllers
{
    public class PurchasesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public PurchasesController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: Purchases
        public async Task<IActionResult> Index()
        {
            var Purchases = await _context.Purchases
                .Include(b => b.Branch)
                .Include(b => b.Branch.Entity)
                .Include(b => b.Customer)
                .ToListAsync();

            var branchesVM = new List<PurchaseFormViewModel>();

            foreach (var purchase in Purchases)
            {
                branchesVM.Add(_mapper.Map<PurchaseFormViewModel>(purchase));
            }

            return View(branchesVM);
        }

        // GET: Purchase by Product Id
        public async Task<IActionResult> ListPurchases(int? id)
        {
            var Purchases = await _context.Purchases
                .Include(b => b.Branch)
                .Include(b => b.Branch.Entity)
                .Include(b => b.Customer)
                .Where(m => m.BranchId == id || m.CustomerId == id)
                .ToListAsync();

            var branchesVM = new List<PurchaseFormViewModel>();

            foreach (var purchase in Purchases)
            {
                branchesVM.Add(_mapper.Map<PurchaseFormViewModel>(purchase));
            }

            return View(branchesVM);
        }

        // GET: Purchases/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchase = await _context.Purchases
                .Include(b => b.Branch)
                .Include(b => b.Branch.Entity)
                .Include(b => b.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (purchase == null)
            {
                return NotFound();
            }

            return View(_mapper.Map<PurchaseFormViewModel>(purchase));
        }

        // GET: Purchases/Create
        public IActionResult Create()
        {
            var purchaseVM = new PurchaseFormViewModel
            {
                Branches = _context.Branches.ToList(),
                Customers = _context.Customers.ToList()
            };

            return View(purchaseVM);
        }

        // POST: Purchases/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PurchaseFormViewModel purchaseVM)
        {
            if (ModelState.IsValid)
            {
                var purchase = _mapper.Map<Purchase>(purchaseVM);
                purchase.EntityId = purchaseVM.Branch.EntityId;
                _context.Add(purchase);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            purchaseVM.Branches = _context.Branches.ToList();
            purchaseVM.Customers = _context.Customers.ToList();

            return View(purchaseVM);
        }

        // GET: Purchases/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchase = await _context.Purchases
                .Include(b => b.Branch)
                .Include(b => b.Branch.Entity)
                .Include(b => b.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (purchase == null)
            {
                return NotFound();
            }

            var purchaseVM = _mapper.Map<PurchaseFormViewModel>(purchase);

            purchaseVM.Branches = _context.Branches.ToList();
            purchaseVM.Customers = _context.Customers.ToList();

            return View(purchaseVM);
        }

        // POST: Purchases/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PurchaseFormViewModel purchaseVM)
        {
            if (id != purchaseVM.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var purchase = _mapper.Map<Purchase>(purchaseVM);
                    purchase.EntityId = purchaseVM.Branch.EntityId;
                    _context.Update(purchase);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await BranchExists(purchaseVM.Id))
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
            purchaseVM.Branches = _context.Branches.ToList();
            purchaseVM.Customers = _context.Customers.ToList();

            return View(purchaseVM);
        }

        // GET: Purchases/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchase = await _context.Purchases
                .Include(b => b.Branch)
                .Include(b => b.Branch.Entity)
                .Include(b => b.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (purchase == null)
            {
                return NotFound();
            }

            return View(_mapper.Map<PurchaseFormViewModel>(purchase));
        }

        // POST: Purchases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var purchase = await _context.Purchases.FindAsync(id);
            _context.Purchases.Remove(purchase);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> BranchExists(int id)
        {
            return await _context.Purchases.AnyAsync(e => e.Id == id);
        }
    }
}