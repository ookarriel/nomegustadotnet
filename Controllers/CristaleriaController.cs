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
    public class CristaleriaController : Controller
    {
        private readonly BarraDbContext _context;

        public CristaleriaController(BarraDbContext context)
        {
            _context = context;
        }

        // GET: Cristaleria
        public async Task<IActionResult> Index()
        {

            return View(await _context.Cristaleria.OrderBy(c => c.NombreCristaleria).ToListAsync());
        }

        // GET: Cristaleria/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cristalerium = await _context.Cristaleria
                .FirstOrDefaultAsync(m => m.IdCristaleria == id);
            if (cristalerium == null)
            {
                return NotFound();
            }

            return View(cristalerium);
        }

        // GET: Cristaleria/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cristaleria/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCristaleria,NombreCristaleria,CapacidadOz")] Cristalerium cristalerium)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cristalerium);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cristalerium);
        }

        // GET: Cristaleria/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cristalerium = await _context.Cristaleria.FindAsync(id);
            if (cristalerium == null)
            {
                return NotFound();
            }
            return View(cristalerium);
        }

        // POST: Cristaleria/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCristaleria,NombreCristaleria,CapacidadOz")] Cristalerium cristalerium)
        {
            if (id != cristalerium.IdCristaleria)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cristalerium);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CristaleriumExists(cristalerium.IdCristaleria))
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
            return View(cristalerium);
        }



        // POST: Cristaleria/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var cristaleria = await _context.Cristaleria.FindAsync(id);
            if (cristaleria != null)
            {
                bool vasoEnUso = _context.Coctels.Any(c => c.IdCristaleria == id);

                if (vasoEnUso)
                {
                    return RedirectToAction(nameof(Index));
                }


                _context.Cristaleria.Remove(cristaleria);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CristaleriumExists(int id)
        {
            return _context.Cristaleria.Any(e => e.IdCristaleria == id);
        }

    }      
}
