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
    public class EntitiesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public EntitiesController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: Entities
        public async Task<IActionResult> Index()
        {
            var entities = await _context.Entities.ToListAsync();
            var entitiesVM = new List<EntityFormViewModel>();

            foreach (var entity in entities)
            {
                entitiesVM.Add(_mapper.Map<EntityFormViewModel>(entity));
            }

            return View(entitiesVM);
        }

        // GET: Entities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entity = await _context.Entities
                .FirstOrDefaultAsync(m => m.Id == id);
            if (entity == null)
            {
                return NotFound();
            }

            return View(_mapper.Map<EntityFormViewModel>(entity));
        }

        // GET: Entities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Entities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EntityFormViewModel entityVM)
        {
            if (ModelState.IsValid)
            {
                var entity = _mapper.Map<Entity>(entityVM);

                _context.Add(entity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(entityVM);
        }

        // GET: Entities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entity = await _context.Entities.FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            return View(_mapper.Map<EntityFormViewModel>(entity));
        }

        // POST: Entities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EntityFormViewModel entityVM)
        {
            if (id != entityVM.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var entity = _mapper.Map<Entity>(entityVM);

                    _context.Update(entity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await EntityExists(entityVM.Id))
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
            return View(entityVM);
        }

        // GET: Entities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entity = await _context.Entities
                .FirstOrDefaultAsync(m => m.Id == id);
            if (entity == null)
            {
                return NotFound();
            }

            return View(_mapper.Map<EntityFormViewModel>(entity));
        }

        // POST: Entities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entity = await _context.Entities.FindAsync(id);
            _context.Entities.Remove(entity);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> EntityExists(int id)
        {
            return await _context.Entities.AnyAsync(e => e.Id == id);
        }
    }
}