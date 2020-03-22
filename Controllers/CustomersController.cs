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
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CustomersController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: Customers
        public async Task<IActionResult> Index()
        {
            var customers = await _context.Customers
                .Include(b => b.Contact)
                .Include(b => b.Entity)
                .ToListAsync();

            return View(_mapper.Map<IEnumerable<CustomerFormViewModel>>(customers));
        }

        // GET: Customers/ListCustomers/5
        public async Task<IActionResult> ListCustomers(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customers = await _context.Customers
                .Include(b => b.Entity)
                .Include(b => b.Contact)
                .Where(m => m.EntityId == id)
                .ToListAsync();

            return View(_mapper.Map<IEnumerable<CustomerFormViewModel>>(customers));
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .Include(b => b.Contact)
                .Include(b => b.Entity)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (customer == null)
            {
                return NotFound();
            }

            return View(_mapper.Map<CustomerFormViewModel>(customer));
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            var customerVM = new CustomerFormViewModel
            {
                Entities = _context.Entities.ToList()
            };

            return View(customerVM);
        }

        // POST: Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerFormViewModel customerVM)
        {
            if (ModelState.IsValid)
            {
                var customer = _mapper.Map<Customer>(customerVM);

                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            customerVM.Entities = _context.Entities.ToList();

            return View(customerVM);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .Include(b => b.Contact)
                .Include(b => b.Entity)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (customer == null)
            {
                return NotFound();
            }

            var customerVM = _mapper.Map<CustomerFormViewModel>(customer);

            customerVM.Entities = _context.Entities.ToList();

            return View(customerVM);
        }

        // POST: Customers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CustomerFormViewModel customerVM)
        {
            if (id != customerVM.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var customer = _mapper.Map<Customer>(customerVM);

                    _context.Update(customer);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await BranchExists(customerVM.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            customerVM.Entities = _context.Entities.ToList();

            return View(customerVM);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .Include(b => b.Contact)
                .Include(b => b.Entity)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(_mapper.Map<CustomerFormViewModel>(customer));
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> BranchExists(int id)
        {
            return await _context.Customers.AnyAsync(e => e.Id == id);
        }
    }
}