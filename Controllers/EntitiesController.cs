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
                var entityVM = new EntityFormViewModel();

                entityVM = _mapper.Map<EntityFormViewModel>(entity);
                //entityVM.Id = entity.Id;
                //entityVM.Name = entity.Name;

                entitiesVM.Add(entityVM);
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

            var entityVM = new EntityFormViewModel();

            entityVM = _mapper.Map<EntityFormViewModel>(entity);

            return View(entityVM);
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
                var entity = new Entity();
                entity.Name = entityVM.Name;

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

            var entityVM = new EntityFormViewModel();

            entityVM = _mapper.Map<EntityFormViewModel>(entity);

            return View(entityVM);
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
                    var entity = new Entity();

                    entity = _mapper.Map<Entity>(entityVM);

                    _context.Update(entity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EntityExists(entityVM.Id))
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

            var entityVM = new EntityFormViewModel();

            entityVM = _mapper.Map<EntityFormViewModel>(entity);

            return View(entityVM);
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

        private bool EntityExists(int id)
        {
            return _context.Entities.Any(e => e.Id == id);
        }
    }
}