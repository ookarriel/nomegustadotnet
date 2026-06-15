using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaManejoBar.Models;
using Microsoft.AspNetCore.Authorization;

namespace SistemaManejoBar.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DetalleRecetasController : Controller
    {
        private readonly BarraDbContext _context;

        public DetalleRecetasController(BarraDbContext context)
        {
            _context = context;
        }

        // GET: DetalleRecetas
        public async Task<IActionResult> Index()
        {
            var barraDbContext = _context.DetalleReceta.Include(d => d.IdCoctelNavigation).Include(d => d.IdIngredienteNavigation);
            return View(await barraDbContext.ToListAsync());
        }

        // GET: DetalleRecetas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detalleRecetum = await _context.DetalleReceta
                .Include(d => d.IdCoctelNavigation)
                .Include(d => d.IdIngredienteNavigation)
                .FirstOrDefaultAsync(m => m.IdDetalleReceta == id);
            if (detalleRecetum == null)
            {
                return NotFound();
            }

            return View(detalleRecetum);
        }

        // GET: DetalleRecetas/Create
        public IActionResult Create()
        {
            ViewData["IdCoctel"] = new SelectList(_context.Coctels, "IdCoctel", "IdCoctel");
            ViewData["IdIngrediente"] = new SelectList(_context.Ingredientes, "IdIngrediente", "IdIngrediente");
            return View();
        }

        // POST: DetalleRecetas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdDetalleReceta,IdCoctel,IdIngrediente,Cantidad")] DetalleRecetum detalleRecetum)
        {
            if (ModelState.IsValid)
            {
                _context.Add(detalleRecetum);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdCoctel"] = new SelectList(_context.Coctels, "IdCoctel", "IdCoctel", detalleRecetum.IdCoctel);
            ViewData["IdIngrediente"] = new SelectList(_context.Ingredientes, "IdIngrediente", "IdIngrediente", detalleRecetum.IdIngrediente);
            return View(detalleRecetum);
        }

        // GET: DetalleRecetas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detalleRecetum = await _context.DetalleReceta.FindAsync(id);
            if (detalleRecetum == null)
            {
                return NotFound();
            }
            ViewData["IdCoctel"] = new SelectList(_context.Coctels, "IdCoctel", "IdCoctel", detalleRecetum.IdCoctel);
            ViewData["IdIngrediente"] = new SelectList(_context.Ingredientes, "IdIngrediente", "IdIngrediente", detalleRecetum.IdIngrediente);
            return View(detalleRecetum);
        }

        // POST: DetalleRecetas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdDetalleReceta,IdCoctel,IdIngrediente,Cantidad")] DetalleRecetum detalleRecetum)
        {
            if (id != detalleRecetum.IdDetalleReceta)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(detalleRecetum);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DetalleRecetumExists(detalleRecetum.IdDetalleReceta))
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
            ViewData["IdCoctel"] = new SelectList(_context.Coctels, "IdCoctel", "IdCoctel", detalleRecetum.IdCoctel);
            ViewData["IdIngrediente"] = new SelectList(_context.Ingredientes, "IdIngrediente", "IdIngrediente", detalleRecetum.IdIngrediente);
            return View(detalleRecetum);
        }

        // GET: DetalleRecetas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detalleRecetum = await _context.DetalleReceta
                .Include(d => d.IdCoctelNavigation)
                .Include(d => d.IdIngredienteNavigation)
                .FirstOrDefaultAsync(m => m.IdDetalleReceta == id);
            if (detalleRecetum == null)
            {
                return NotFound();
            }

            return View(detalleRecetum);
        }

        // POST: DetalleRecetas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var detalleRecetum = await _context.DetalleReceta.FindAsync(id);
            if (detalleRecetum != null)
            {
                _context.DetalleReceta.Remove(detalleRecetum);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DetalleRecetumExists(int id)
        {
            return _context.DetalleReceta.Any(e => e.IdDetalleReceta == id);
        }
    }
}
