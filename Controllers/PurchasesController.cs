using System;
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
    public class PurchasesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly int _entityId;

        public PurchasesController(ApplicationDbContext context, IMapper mapper, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IHttpContextAccessor httpContextAccessor)
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

        // GET: Purchases
        public async Task<IActionResult> Index()
        {
            var purchases = await _context.Purchases
                .Where(m => m.EntityId == _entityId)
                .Include(b => b.Branch)
                    .ThenInclude(b => b.Entity)
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
                .Where(m => m.EntityId == _entityId && m.BranchId == id)
                .Include(b => b.Branch)
                    .ThenInclude(b => b.Entity)
                .Include(b => b.Customer)
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
                .Where(m => m.EntityId == _entityId && m.CustomerId == id)
                .Include(b => b.Branch)
                    .ThenInclude(b => b.Entity)
                .Include(b => b.Customer)
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
                    .ThenInclude(b => b.Entity)
                .Include(b => b.Customer)
                .Include(b => b.PurchaseItems)
                .SingleOrDefaultAsync(m => m.EntityId == _entityId && m.Id == id);

            if (purchase == null)
            {
                return NotFound();
            }

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
                Branches = _context.Branches.Where(m => m.EntityId == _entityId).ToList(),
                Customers = _context.Customers.Where(m => m.EntityId == _entityId).ToList(),
                Products = _context.Products.Where(m => m.EntityId == _entityId).ToList()
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
                    .SingleOrDefaultAsync(m => m.EntityId == _entityId && m.Id == purchaseVM.BranchId);

                purchaseVM.EntityId = branch.EntityId;

                string timestamp = DateTime.Now.Ticks.ToString();
                Random randonNum = new Random();

                purchaseVM.Identification = timestamp + randonNum.Next(1, 9999999);

                purchaseVM.RegistrationDate = DateTime.Now;

                var purchase = _mapper.Map<Purchase>(purchaseVM);

                foreach (var purchaseitem in purchase.PurchaseItems)
                {
                    purchase.Discount += purchaseitem.DiscountTotal;
                    purchase.Total += purchaseitem.Total - purchaseitem.DiscountTotal;
                    _context.Add(purchaseitem);

                    var product = await _context.Products
                    .SingleOrDefaultAsync(m => m.Id == purchaseitem.ProductId);

                    product.QuantityInStock -= purchaseitem.ProductQuantity;

                    _context.Entry(product).Property("QuantityInStock").IsModified = true;
                }

                _context.Add(purchase);

                await _context.SaveChangesAsync();

                return Json(null);
            }
            purchaseVM.Branches = _context.Branches.Where(m => m.EntityId == _entityId).ToList();
            purchaseVM.Customers = _context.Customers.Where(m => m.EntityId == _entityId).ToList();
            purchaseVM.Products = _context.Products.Where(m => m.EntityId == _entityId).ToList();

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
                    .ThenInclude(b => b.Entity)
                .Include(b => b.Customer)
                .SingleOrDefaultAsync(m => m.EntityId == _entityId && m.Id == id);

            if (purchase == null)
            {
                return NotFound();
            }

            var purchaseVM = _mapper.Map<PurchaseFormViewModel>(purchase);

            purchaseVM.Branches = _context.Branches.Where(m => m.EntityId == _entityId).ToList();
            purchaseVM.Customers = _context.Customers.Where(m => m.EntityId == _entityId).ToList();

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
                        .SingleOrDefaultAsync(m => m.EntityId == _entityId && m.Id == purchaseVM.BranchId);
                    purchaseVM.EntityId = branch.EntityId;

                    var purchase = _mapper.Map<Purchase>(purchaseVM);

                    _context.Entry(purchase).Property("BranchId").IsModified = true;
                    _context.Entry(purchase).Property("CustomerId").IsModified = true;
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
            purchaseVM.Branches = _context.Branches.Where(m => m.EntityId == _entityId).ToList();
            purchaseVM.Customers = _context.Customers.Where(m => m.EntityId == _entityId).ToList();

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
                    .ThenInclude(b => b.Entity)
                .Include(b => b.Customer)
               .SingleOrDefaultAsync(m => m.EntityId == _entityId && m.Id == id);
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