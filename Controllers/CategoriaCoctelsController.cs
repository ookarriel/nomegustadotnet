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
    public class CategoriaCoctelsController : Controller
    {
        private readonly BarraDbContext _context;

        public CategoriaCoctelsController(BarraDbContext context)
        {
            _context = context;
        }

        // GET: CategoriaCoctels
        public async Task<IActionResult> Index()
        {
            return View(await _context.CategoriaCoctels.ToListAsync());
        }

        // GET: CategoriaCoctels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var categoriaCoctel = await _context.CategoriaCoctels
                .FirstOrDefaultAsync(m => m.IdCategoria == id);
            if (categoriaCoctel == null) return NotFound();

            return View(categoriaCoctel);
        }

        // GET: CategoriaCoctels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CategoriaCoctels/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCategoria,NombreCategoria,Descripcion")] CategoriaCoctel categoriaCoctel)
        {
            
            ModelState.Remove("Coctels");

            if (ModelState.IsValid)
            {
                _context.Add(categoriaCoctel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(categoriaCoctel);
        }

        // GET: CategoriaCoctels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var categoriaCoctel = await _context.CategoriaCoctels.FindAsync(id);
            if (categoriaCoctel == null) return NotFound();

            return View(categoriaCoctel);
        }

        // POST: CategoriaCoctels/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCategoria,NombreCategoria,Descripcion")] CategoriaCoctel categoriaCoctel)
        {
            if (id != categoriaCoctel.IdCategoria) return NotFound();

          
            ModelState.Remove("Coctels");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categoriaCoctel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoriaCoctelExists(categoriaCoctel.IdCategoria))
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
            return View(categoriaCoctel);
        }

     
        // POST: CategoriaCoctels/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var categoria = await _context.CategoriaCoctels.FindAsync(id);
            if (categoria != null)
            {
                bool categoriaEnUso = _context.Coctels.Any(c => c.IdCategoria == id);

                if (categoriaEnUso)
                {
                    
                    TempData["Error"] = $"No se puede eliminar la categoría '{categoria.NombreCategoria}' porque tiene cócteles asociados. Elimina o cambia de categoría esos cócteles primero.";
                    return RedirectToAction(nameof(Index));
                }

                _context.CategoriaCoctels.Remove(categoria);
                await _context.SaveChangesAsync();
    
                TempData["Exito"] = "Categoría eliminada correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CategoriaCoctelExists(int id)
        {
            return _context.CategoriaCoctels.Any(e => e.IdCategoria == id);
        }
    }
}