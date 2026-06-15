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
    public class TipoIngredientesController : Controller
    {
        private readonly BarraDbContext _context;

        public TipoIngredientesController(BarraDbContext context)
        {
            _context = context;
        }

        // GET: TipoIngredientes
        public async Task<IActionResult> Index()
        {
            return View(await _context.TipoIngredientes.ToListAsync());
        }

        // GET: TipoIngredientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoIngrediente = await _context.TipoIngredientes
                .FirstOrDefaultAsync(m => m.IdTipoIngrediente == id);
            if (tipoIngrediente == null)
            {
                return NotFound();
            }

            return View(tipoIngrediente);
        }

        // GET: TipoIngredientes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TipoIngredientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdTipoIngrediente,NombreTipoIngrediente")] TipoIngrediente tipoIngrediente)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tipoIngrediente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tipoIngrediente);
        }

        // GET: TipoIngredientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoIngrediente = await _context.TipoIngredientes.FindAsync(id);
            if (tipoIngrediente == null)
            {
                return NotFound();
            }
            return View(tipoIngrediente);
        }

        // POST: TipoIngredientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdTipoIngrediente,NombreTipoIngrediente")] TipoIngrediente tipoIngrediente)
        {
            if (id != tipoIngrediente.IdTipoIngrediente)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipoIngrediente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoIngredienteExists(tipoIngrediente.IdTipoIngrediente))
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
            return View(tipoIngrediente);
        }

        // GET: TipoIngredientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoIngrediente = await _context.TipoIngredientes
                .FirstOrDefaultAsync(m => m.IdTipoIngrediente == id);
            if (tipoIngrediente == null)
            {
                return NotFound();
            }

            return View(tipoIngrediente);
        }

        // POST: TipoIngredientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tipoIngrediente = await _context.TipoIngredientes.FindAsync(id);
            if (tipoIngrediente != null)
            {
                _context.TipoIngredientes.Remove(tipoIngrediente);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TipoIngredienteExists(int id)
        {
            return _context.TipoIngredientes.Any(e => e.IdTipoIngrediente == id);
        }
    }
}
