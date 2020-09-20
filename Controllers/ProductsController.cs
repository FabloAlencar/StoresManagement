﻿using System;
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
using StoresManagement.Constants;
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
        private readonly List<int> _entityIds = new List<int>();

        public ProductsController(ApplicationDbContext context, IMapper mapper, IWebHostEnvironment hostEnvironment, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _hostEnvironment = hostEnvironment;
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

        // GET: Products/GetProducts
        [HttpGet]
        public ActionResult GetProducts()
        {
            var productList = _context.Products
                .Where(b => _entityIds.Contains(b.EntityId))
                .Select(r => new
                {
                    productId = r.Id,
                    productBranch = r.Branch.Entity.Name + ", " + r.Branch.Name,
                    productProduct = r.Name + ", " + r.Brand,
                    productQuantityInStock = r.QuantityInStock,
                    productPrice = r.Price,
                    productExpiryDate = Convert.ToDateTime(r.ExpiryDate).ToString("dd-MMM-yyyy"),
                    productWeight = r.Weight,
                    productWidth = r.Width,
                    productHeight = r.Height
                }).ToArray();

            var dataPage = new
            {
                last_page = 0,
                data = productList
            };

            return Json(dataPage);
        }

        // GET: Products
        public IActionResult Index()
        {
            return View();
        }

        // GET: Products/ListProductsByBranch/5
        public async Task<IActionResult> ListProductsByBranch(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products
                .Where(m => _entityIds.Contains(m.EntityId) && m.BranchId == id)
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
                .SingleOrDefaultAsync(m => _entityIds.Contains(m.EntityId) && m.Id == id);

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
                .Where(m => _entityIds.Contains(m.EntityId))
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
                        .SingleOrDefaultAsync(m => _entityIds.Contains(m.EntityId) && m.Id == productVM.BranchId);
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
        [Authorize(Roles = "Manager,Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);

            product.Active = false;
            _context.Entry(product).Property("Active").IsModified = true;

            await _context.SaveChangesAsync();

            // Delete image from wwwRootPath/image
            DeleteImageFromwwwRootPath(product.ImageName);

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