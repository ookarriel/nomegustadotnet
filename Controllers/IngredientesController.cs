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
    public class IngredientesController : Controller
    {
        private readonly BarraDbContext _context;

        public IngredientesController(BarraDbContext context)
        {
            _context = context;
        }

        // GET: Ingredientes
        public async Task<IActionResult> Index()
        {
            var barraDbContext = _context.Ingredientes.Include(i => i.IdTipoIngredienteNavigation);
            return View(await barraDbContext.ToListAsync());
        }

        // GET: Ingredientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingrediente = await _context.Ingredientes
                .Include(i => i.IdTipoIngredienteNavigation)
                .FirstOrDefaultAsync(m => m.IdIngrediente == id);
            if (ingrediente == null)
            {
                return NotFound();
            }

            return View(ingrediente);
        }

        // GET: Ingredientes/Create
        public IActionResult Create()
        {
            ViewData["IdTipoIngrediente"] = new SelectList(_context.TipoIngredientes, "IdTipoIngrediente", "NombreTipoIngrediente");
            return View();
        }

        // POST: Ingredientes/Create
        // POST: Ingredientes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdIngrediente,NombreIngrediente,Stock,Unidad,IdTipoIngrediente")] Ingrediente ingrediente)
        {
            // PARCHE: Ignorar validación de las relaciones
            ModelState.Remove("IdTipoIngredienteNavigation");
            ModelState.Remove("DetalleReceta");

            if (ModelState.IsValid)
            {
                bool existeIngrediente = await _context.Ingredientes
                    .AnyAsync(i => i.NombreIngrediente.ToLower().Trim() == ingrediente.NombreIngrediente.ToLower().Trim());

                if (existeIngrediente)
                {
                
                    ModelState.AddModelError("NombreIngrediente", "Ya existe un ingrediente con este nombre en la barra.");

                    ViewData["IdTipoIngrediente"] = new SelectList(_context.TipoIngredientes, "IdTipoIngrediente", "NombreTipoIngrediente", ingrediente.IdTipoIngrediente);
                    return View(ingrediente);
                }
              

                _context.Add(ingrediente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdTipoIngrediente"] = new SelectList(_context.TipoIngredientes, "IdTipoIngrediente", "NombreTipoIngrediente", ingrediente.IdTipoIngrediente);
            return View(ingrediente);
        }

        // GET: Ingredientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingrediente = await _context.Ingredientes.FindAsync(id);
            if (ingrediente == null)
            {
                return NotFound();
            }
            ViewData["IdTipoIngrediente"] = new SelectList(_context.TipoIngredientes, "IdTipoIngrediente", "NombreTipoIngrediente", ingrediente.IdTipoIngrediente);
            return View(ingrediente);
        }

        // POST: Ingredientes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdIngrediente,NombreIngrediente,Stock,Unidad,IdTipoIngrediente")] Ingrediente ingrediente)
        {
            if (id != ingrediente.IdIngrediente)
            {
                return NotFound();
            }

         
            ModelState.Remove("IdTipoIngredienteNavigation");
            ModelState.Remove("DetalleReceta");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ingrediente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IngredienteExists(ingrediente.IdIngrediente))
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
            ViewData["IdTipoIngrediente"] = new SelectList(_context.TipoIngredientes, "IdTipoIngrediente", "NombreTipoIngrediente", ingrediente.IdTipoIngrediente);
            return View(ingrediente);
        }

        // POST: Ingredientes/Delete/5 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var ingrediente = await _context.Ingredientes.FindAsync(id);
            if (ingrediente != null)
            {
               
                var detallesAsociados = _context.DetalleReceta.Where(d => d.IdIngrediente == id);
                _context.DetalleReceta.RemoveRange(detallesAsociados);

              
                _context.Ingredientes.Remove(ingrediente);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool IngredienteExists(int id)
        {
            return _context.Ingredientes.Any(e => e.IdIngrediente == id);
        }
    }
}