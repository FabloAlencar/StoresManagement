using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
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
    [AuthorizeRoles(UserRoles.Manager, UserRoles.Administrator, UserRoles.Seller)]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly List<int> _entityIds = new List<int>();

        public ProductsController(ApplicationDbContext context,
            IMapper mapper,
            IWebHostEnvironment hostEnvironment,
            UserManager<IdentityUser> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _hostEnvironment = hostEnvironment;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _entityIds = AuthorizeEntitiesAttribute.GetEntityIds(_context, _userManager, _httpContextAccessor);
        }

        // GET: Products/Search
        [HttpGet]
        public ActionResult Search(string term)
        {
            var productList = _context.Products
                .Where(b => _entityIds.Contains(b.EntityId)
                && b.Active == true
                && b.QuantityInStock > 0
                && (b.Name.Contains(term) || b.Brand.Contains(term)))
                              .Select(r => new
                              {
                                  productId = r.Id,
                                  productName = r.Name + ", " + r.Brand,
                                  productPrice = r.Price,
                                  productQuantityInStock = r.QuantityInStock
                              }).ToArray();

            return Json(productList);
        }

        // GET: Products/ListAll
        [HttpGet]
        public ActionResult ListAll()
        {
            var list = _context.Products
                .Where(b => _entityIds.Contains(b.EntityId))
                .Select(r => new
                {
                    id = r.Id,
                    branch = r.Branch.Entity.Name + ", " + r.Branch.Name,
                    product = r.Name + ", " + r.Brand,
                    quantityInStock = r.QuantityInStock,
                    price = r.Price,
                    expiryDate = Convert.ToDateTime(r.ExpiryDate).ToString("dd-MMM-yyyy"),
                    weight = r.Weight,
                    width = r.Width,
                    height = r.Height,
                    active = r.Active
                }).ToArray();

            return Json(new
            {
                last_page = 0,
                data = list
            });
        }

        // GET: Products
        public IActionResult Index()
        {
            return View();
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(b => b.Branch)
                    .ThenInclude(b => b.Entity)
                .SingleOrDefaultAsync(m => _entityIds.Contains(m.EntityId) && m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(_mapper.Map<ProductFormViewModel>(product));
        }

        // GET: Products/Create
        [AuthorizeRoles(UserRoles.Manager, UserRoles.Administrator)]
        public IActionResult Create()
        {
            var productVM = new ProductFormViewModel
            {
                Branches = _context.Branches
                .Where(m => _entityIds.Contains(m.EntityId))
                .ToList()
            };

            return View(productVM);
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(UserRoles.Manager, UserRoles.Administrator)]
        public async Task<IActionResult> Create(ProductFormViewModel productVM)
        {
            if (ModelState.IsValid)
            {
                // Save image to wwwRootPath/image
                await AddImageFileAsync(productVM);

                var branch = await _context.Branches
                    .SingleOrDefaultAsync(m => _entityIds.Contains(m.EntityId) && m.Id == productVM.BranchId);
                productVM.EntityId = branch.EntityId;

                var product = _mapper.Map<Product>(productVM);

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            productVM.Branches = _context.Branches
                .Where(m => _entityIds.Contains(m.EntityId))
                .ToList();

            return View(productVM);
        }

        // GET: Products/Edit/5
        [AuthorizeRoles(UserRoles.Manager, UserRoles.Administrator)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(b => b.Branch)
                    .ThenInclude(b => b.Entity)
                .SingleOrDefaultAsync(m => _entityIds.Contains(m.EntityId) && m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            var productVM = _mapper.Map<ProductFormViewModel>(product);

            productVM.Branches = _context.Branches
                .Where(m => _entityIds.Contains(m.EntityId))
                .ToList();

            return View(productVM);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(UserRoles.Manager, UserRoles.Administrator)]
        public async Task<IActionResult> Edit(int id, ProductFormViewModel productVM)
        {
            if (id != productVM.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var branch = await _context.Branches
                        .SingleOrDefaultAsync(m => _entityIds.Contains(m.EntityId) && m.Id == productVM.BranchId);
                    productVM.EntityId = branch.EntityId;

                    // Delete image from wwwRootPath/image && Save image to wwwRootPath/image
                    if (productVM.ImageFile != null)
                        DeleteImageFile(productVM.ImageName);
                    await AddImageFileAsync(productVM);

                    var product = _mapper.Map<Product>(productVM);

                    _context.Update(product);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await BranchExists(productVM.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            productVM.Branches = _context.Branches.ToList();

            return View(productVM);
        }

        // GET: Products/Delete/5
        [AuthorizeRoles(UserRoles.Manager, UserRoles.Administrator)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(b => b.Branch)
                    .ThenInclude(b => b.Entity)
                .SingleOrDefaultAsync(m => _entityIds.Contains(m.EntityId) && m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(_mapper.Map<ProductFormViewModel>(product));
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeRoles(UserRoles.Manager, UserRoles.Administrator)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);

            product.Active = false;
            _context.Entry(product).Property("Active").IsModified = true;

            await _context.SaveChangesAsync();

            // Delete image from wwwRootPath/image
            DeleteImageFile(product.ImageName);

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> BranchExists(int id)
        {
            return await _context.Products.AnyAsync(e => e.Id == id);
        }

        private async Task AddImageFileAsync(ProductFormViewModel productVM)
        {
            if (productVM.ImageFile == null)
            {
                return;
            }

            string imageExtension = Path.GetExtension(productVM.ImageFile.FileName);
            productVM.ImageName = productVM.Name.Replace(" ", "_") + productVM.Brand.Replace(" ", "_")
                                + DateTime.Now.ToString("_yyyymmddss_fff") + imageExtension;
            string imagePath = Path.Combine(_hostEnvironment.WebRootPath + "/image/", productVM.ImageName);
            using var fileStream = new FileStream(imagePath, FileMode.Create);
            await productVM.ImageFile.CopyToAsync(fileStream);
        }

        private void DeleteImageFile(string imageName)
        {
            if (string.IsNullOrEmpty(imageName))
            {
                return;
            }
            var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "image", imageName);
            if (System.IO.File.Exists(imagePath))
                System.IO.File.Delete(imagePath);
        }
    }
}