﻿using System.Collections.Generic;
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
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProductsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: Products/Search
        [HttpGet]
        public ActionResult Search(string term)
        {
            var productList = _context.Products.Where(r => (r.Name.Contains(term) || r.Brand.Contains(term)) && r.QuantityInStock > 0)
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
                .Include(b => b.Branch)
                    .ThenInclude(b => b.Entity)
                .Where(m => m.BranchId == id)
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
                .SingleOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(_mapper.Map<ProductFormViewModel>(product));
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            var productVM = new ProductFormViewModel
            {
                Branches = _context.Branches.ToList()
            };

            return View(productVM);
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductFormViewModel productVM)
        {
            if (ModelState.IsValid)
            {
                var branch = await _context.Branches
                    .SingleOrDefaultAsync(m => m.Id == productVM.BranchId);
                productVM.EntityId = branch.EntityId;

                var product = _mapper.Map<Product>(productVM);

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            productVM.Branches = _context.Branches.ToList();

            return View(productVM);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(b => b.Branch)
                    .ThenInclude(b => b.Entity)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            var productVM = _mapper.Map<ProductFormViewModel>(product);

            productVM.Branches = _context.Branches.ToList();

            return View(productVM);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
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
                        .SingleOrDefaultAsync(m => m.Id == productVM.BranchId);
                    productVM.EntityId = branch.EntityId;

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
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(b => b.Branch)
                    .ThenInclude(b => b.Entity)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(_mapper.Map<ProductFormViewModel>(product));
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> BranchExists(int id)
        {
            return await _context.Products.AnyAsync(e => e.Id == id);
        }
    }
}