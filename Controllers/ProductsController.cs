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
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProductsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var Products = await _context.Products
                .Include(b => b.Branch)
                .Include(b => b.Branch.Entity)
                .ToListAsync();

            var branchesVM = new List<ProductFormViewModel>();

            foreach (var product in Products)
            {
                branchesVM.Add(_mapper.Map<ProductFormViewModel>(product));
            }

            return View(branchesVM);
        }

        // GET: Products by product Id
        public async Task<IActionResult> ListProducts(int? id)
        {
            var Products = await _context.Products
                .Include(b => b.Branch)
                .Include(b => b.Branch.Entity)
                .Where(m => m.BranchId == id)
                .ToListAsync();

            var branchesVM = new List<ProductFormViewModel>();

            foreach (var product in Products)
            {
                branchesVM.Add(_mapper.Map<ProductFormViewModel>(product));
            }

            return View(branchesVM);
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
                .Include(b => b.Branch.Entity)
                .FirstOrDefaultAsync(m => m.Id == id);

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
                .Include(b => b.Branch.Entity)
                .FirstOrDefaultAsync(m => m.Id == id);

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
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var product = _mapper.Map<Product>(productVM);
                    _context.Update(product);
                    await _context.SaveChangesAsync();
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
                return RedirectToAction(nameof(Index));
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
                .Include(b => b.Branch.Entity)
                .FirstOrDefaultAsync(m => m.Id == id);
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