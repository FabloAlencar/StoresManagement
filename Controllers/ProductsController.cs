using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly int _entityId;

        public ProductsController(ApplicationDbContext context, IMapper mapper, IWebHostEnvironment hostEnvironment, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _hostEnvironment = hostEnvironment;
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

        // GET: Products/Search
        [HttpGet]
        public ActionResult Search(string term)
        {
            var productList = _context.Products
                .Where(r => r.EntityId == _entityId
                && r.QuantityInStock > 0
                && (r.Name.Contains(term) || r.Brand.Contains(term)))
                              .Select(r => new
                              {
                                  productId = r.Id,
                                  productName = r.Name + ", " + r.Brand,
                                  productPrice = r.Price,
                                  productQuantityInStock = r.QuantityInStock
                              }).ToArray();

            return Json(productList);
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Where(m => m.EntityId == _entityId)
                .Include(b => b.Branch)
                    .ThenInclude(b => b.Entity)
                .ToListAsync();

            return View(_mapper.Map<IEnumerable<ProductFormViewModel>>(products));
        }

        // GET: Products/ListProducts/5
        public async Task<IActionResult> ListProducts(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products
                .Where(m => m.EntityId == _entityId && m.BranchId == id)
                .Include(b => b.Branch)
                    .ThenInclude(b => b.Entity)
                .ToListAsync();

            return View(_mapper.Map<IEnumerable<ProductFormViewModel>>(products));
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
                .SingleOrDefaultAsync(m => m.EntityId == _entityId && m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(_mapper.Map<ProductFormViewModel>(product));
        }

        // GET: Products/Create
        [Authorize(Roles = "Manager,Administrator")]
        public IActionResult Create()
        {
            var productVM = new ProductFormViewModel
            {
                Branches = _context.Branches
                .Where(m => m.EntityId == _entityId)
                .ToList()
            };

            return View(productVM);
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager,Administrator")]
        public async Task<IActionResult> Create(ProductFormViewModel productVM)
        {
            if (ModelState.IsValid)
            {
                // Save image to wwwRootPath/image
                await addImageTowwwRootPathAsync(productVM);

                var branch = await _context.Branches
                    .SingleOrDefaultAsync(m => m.EntityId == _entityId && m.Id == productVM.BranchId);
                productVM.EntityId = branch.EntityId;

                var product = _mapper.Map<Product>(productVM);

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            productVM.Branches = _context.Branches
                .Where(m => m.EntityId == _entityId)
                .ToList();

            return View(productVM);
        }

        // GET: Products/Edit/5
        [Authorize(Roles = "Manager,Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(b => b.Branch)
                    .ThenInclude(b => b.Entity)
                .SingleOrDefaultAsync(m => m.EntityId == _entityId && m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            var productVM = _mapper.Map<ProductFormViewModel>(product);

            productVM.Branches = _context.Branches
                .Where(m => m.EntityId == _entityId)
                .ToList();

            return View(productVM);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager,Administrator")]
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
                        .SingleOrDefaultAsync(m => m.EntityId == _entityId && m.Id == productVM.BranchId);
                    productVM.EntityId = branch.EntityId;

                    // Delete image from wwwRootPath/image && Save image to wwwRootPath/image
                    if (productVM.ImageFile != null)
                        DeleteImageFromwwwRootPath(productVM.ImageName);
                    await addImageTowwwRootPathAsync(productVM);

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
        [Authorize(Roles = "Manager,Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(b => b.Branch)
                    .ThenInclude(b => b.Entity)
                .SingleOrDefaultAsync(m => m.EntityId == _entityId && m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(_mapper.Map<ProductFormViewModel>(product));
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager,Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);

            // Delete image from wwwRootPath/image
            DeleteImageFromwwwRootPath(product.ImageName);

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> BranchExists(int id)
        {
            return await _context.Products.AnyAsync(e => e.Id == id);
        }

        private async Task addImageTowwwRootPathAsync(ProductFormViewModel productVM)
        {
            if (productVM.ImageFile != null)
            {
                string imageExtension = Path.GetExtension(productVM.ImageFile.FileName);
                productVM.ImageName = productVM.Name.Replace(" ", "_") + productVM.Brand.Replace(" ", "_")
                                    + DateTime.Now.ToString("_yyyymmddss_fff") + imageExtension;
                string imagePath = Path.Combine(_hostEnvironment.WebRootPath + "/image/", productVM.ImageName);
                using (var fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    await productVM.ImageFile.CopyToAsync(fileStream);
                }
            }
        }

        private void DeleteImageFromwwwRootPath(string imageName)
        {
            if (imageName != null)
            {
                var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "image", imageName);
                if (System.IO.File.Exists(imagePath))
                    System.IO.File.Delete(imagePath);
            }
        }
    }
}