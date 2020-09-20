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
using StoresManagement.Constants;
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
        private readonly List<int> _entityIds = new List<int>();

        public PurchasesController(ApplicationDbContext context, IMapper mapper, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IHttpContextAccessor httpContextAccessor)
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

            var entityUser = _context.Operators.SingleOrDefault(m => m.UserId == _userManager.GetUserId(_httpContextAccessor.HttpContext.User));

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

        // GET: Purchases/ListPurchasesByBranch/5
        public async Task<IActionResult> ListPurchasesByBranch(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchases = await _context.Purchases
                .Where(m => _entityIds.Contains(m.EntityId) && m.BranchId == id)
                .Include(b => b.Branch)
                    .ThenInclude(b => b.Entity)
                .Include(b => b.Customer)
                .ToListAsync();

            return View("ListPurchases", _mapper.Map<IEnumerable<PurchaseFormViewModel>>(purchases));
        }

        // GET: Purchases/ListPurchasesByCustumer/5
        public async Task<IActionResult> ListPurchasesByCustumer(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchases = await _context.Purchases
                .Where(m => _entityIds.Contains(m.EntityId) && m.CustomerId == id)
                .Include(b => b.Branch)
                    .ThenInclude(b => b.Entity)
                .Include(b => b.Customer)
                .ToListAsync();

            return View("ListPurchases", _mapper.Map<IEnumerable<PurchaseFormViewModel>>(purchases));
        }

        // GET: Purchases/ListAll
        [HttpGet]
        public ActionResult ListAll()
        {
            var list = _context.Purchases
                .Where(b => _entityIds.Contains(b.EntityId))
                .Select(r => new
                {
                    id = r.Id,
                    purchaseBranch = r.Branch.Entity.Name + ", " + r.Branch.Name,
                    purchaseCustomer = r.Customer.FullName,
                    purchaseDate = Convert.ToDateTime(r.RegistrationDate).ToString("dd-MMM-yyyy"),
                    purchaseDiscount = r.Discount,
                    purchaseTotal = r.Total
                }).ToArray();

            var dataPage = new
            {
                last_page = 0,
                data = list
            };

            return Json(dataPage);
        }

        // GET: Purchases
        public async Task<IActionResult> Index()
        {
            var purchases = await _context.Purchases
                .Where(m => _entityIds.Contains(m.EntityId))
                .Include(b => b.Branch)
                    .ThenInclude(b => b.Entity)
                .Include(b => b.Customer)
                .ToListAsync();

            return View(_mapper.Map<IEnumerable<PurchaseFormViewModel>>(purchases));
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
                    .ThenInclude(b => b.Product)
                .SingleOrDefaultAsync(m => _entityIds.Contains(m.EntityId) && m.Id == id);

            if (purchase == null)
            {
                return NotFound();
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
                Branches = _context.Branches
                .Where(m => m.Active == true
                && _entityIds.Contains(m.EntityId))
                .ToList()
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
                    .SingleOrDefaultAsync(m => _entityIds.Contains(m.EntityId) && m.Id == purchaseVM.BranchId);

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
                    purchaseitem.EntityId = purchase.EntityId;
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
            purchaseVM.Branches = _context.Branches.Where(m => _entityIds.Contains(m.EntityId)).ToList();
            purchaseVM.Customers = _context.Customers.Where(m => _entityIds.Contains(m.EntityId)).ToList();
            purchaseVM.Products = _context.Products.Where(m => _entityIds.Contains(m.EntityId)).ToList();

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
                .SingleOrDefaultAsync(m => _entityIds.Contains(m.EntityId) && m.Id == id);

            if (purchase == null)
            {
                return NotFound();
            }

            var purchaseVM = _mapper.Map<PurchaseFormViewModel>(purchase);

            purchaseVM.Branches = _context.Branches.Where(m => _entityIds.Contains(m.EntityId)).ToList();
            purchaseVM.Customers = _context.Customers.Where(m => _entityIds.Contains(m.EntityId)).ToList();

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
                        .SingleOrDefaultAsync(m => _entityIds.Contains(m.EntityId) && m.Id == purchaseVM.BranchId);
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
            purchaseVM.Branches = _context.Branches.Where(m => _entityIds.Contains(m.EntityId)).ToList();
            purchaseVM.Customers = _context.Customers.Where(m => _entityIds.Contains(m.EntityId)).ToList();

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
               .SingleOrDefaultAsync(m => _entityIds.Contains(m.EntityId) && m.Id == id);

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

            var purchaseItems = _context.PurchaseItems
                .Include(m => m.Product)
                .Where(m => m.PurchaseId == id)
                .ToList();

            _context.Purchases.Remove(purchase);

            foreach (var purchaseitem in purchaseItems)
            {
                purchaseitem.Product.QuantityInStock += purchaseitem.ProductQuantity;

                _context.Entry(purchaseitem.Product).Property("QuantityInStock").IsModified = true;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> BranchExists(int id)
        {
            return await _context.Purchases.AnyAsync(e => e.Id == id);
        }
    }
}