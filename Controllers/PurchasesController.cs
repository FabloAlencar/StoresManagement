using System;
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
            var purchases = await _context.Purchases
                .Include(b => b.Branch)
                .Include(b => b.Branch.Entity)
                .Include(b => b.Customer)
                .ToListAsync();

            return View(_mapper.Map<IEnumerable<PurchaseFormViewModel>>(purchases));
        }

        // GET: Purchases/BranchListOfPurchases/5
        public async Task<IActionResult> BranchListOfPurchases(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchases = await _context.Purchases
                .Include(b => b.Branch)
                .Include(b => b.Branch.Entity)
                .Include(b => b.Customer)
                .Where(m => m.BranchId == id)
                .ToListAsync();

            return View("ListPurchases", _mapper.Map<IEnumerable<PurchaseFormViewModel>>(purchases));
        }

        // GET: Purchases/CustomerListOfPurchases/5
        public async Task<IActionResult> CustomerListOfPurchases(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchases = await _context.Purchases
                .Include(b => b.Branch)
                .Include(b => b.Branch.Entity)
                .Include(b => b.Customer)
                .Where(m => m.CustomerId == id)
                .ToListAsync();

            return View("ListPurchases", _mapper.Map<IEnumerable<PurchaseFormViewModel>>(purchases));
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
                .Include(b => b.PurchaseItems)
                .SingleOrDefaultAsync(m => m.Id == id);

            foreach (var item in purchase.PurchaseItems)
            {
                item.Product = await _context.Products
                    .SingleOrDefaultAsync(m => m.Id == item.ProductId);
            }

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
                Customers = _context.Customers.ToList(),
                Products = _context.Products.ToList()
            };

            return View(purchaseVM);
        }

        // POST: Purchases/Create
        [HttpPost]
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] PurchaseFormViewModel purchaseVM)
        {
            if (ModelState.IsValid)
            {
                var branch = await _context.Branches
                    .SingleOrDefaultAsync(m => m.Id == purchaseVM.BranchId);

                purchaseVM.EntityId = branch.EntityId;
                purchaseVM.Identification = DateTime.Now.ToString("yyMMddHHmmss0000ffff");
                purchaseVM.RegistrationDate = DateTime.Now;

                var purchase = _mapper.Map<Purchase>(purchaseVM);

                foreach (var purchaseitem in purchase.PurchaseItems)
                {
                    purchase.Discount += purchaseitem.DiscountTotal;
                    purchase.Total += (purchaseitem.Total - purchaseitem.DiscountTotal);
                    _context.Add(purchaseitem);
                }

                _context.Add(purchase);

                await _context.SaveChangesAsync();

                return Json(null);
                //  return Json(RedirectToAction(nameof(Index)));
            }
            purchaseVM.Branches = _context.Branches.ToList();
            purchaseVM.Customers = _context.Customers.ToList();
            purchaseVM.Products = _context.Products.ToList();

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
                .SingleOrDefaultAsync(m => m.Id == id);

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
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var branch = await _context.Branches
                        .SingleOrDefaultAsync(m => m.Id == purchaseVM.BranchId);
                    purchaseVM.EntityId = branch.EntityId;

                    var purchase = _mapper.Map<Purchase>(purchaseVM);
                    _context.Update(purchase);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
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
               .SingleOrDefaultAsync(m => m.Id == id);
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