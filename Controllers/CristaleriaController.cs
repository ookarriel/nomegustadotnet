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
    // Controlador para gestionar el inventario de cristalería de la barra
    [Authorize(Roles = "Administrador,Bartender")]
    public class CristaleriaController : Controller
    {
        private readonly BarraDbContext _context;

        public CristaleriaController(BarraDbContext context)
        {
            _context = context;
        }

        // GET: Cristaleria (Con Búsqueda, Ordenación y Paginación)
        public async Task<IActionResult> Index(string? buscar, string? orden, int? pagina)
        {
            // Persistir los filtros en la vista
            ViewData["BuscarActual"] = buscar;
            ViewData["OrdenActual"] = orden;

            // Parámetros de ordenación
            ViewData["NombreSortParam"] = string.IsNullOrEmpty(orden) ? "nombre_desc" : "";
            ViewData["CapacidadSortParam"] = orden == "capacidad_asc" ? "capacidad_desc" : "capacidad_asc";

            var consulta = _context.Cristaleria.AsQueryable();

            // 1. FILTRO / BUSQUEDA (al menos 2 campos)
            if (!string.IsNullOrEmpty(buscar))
            {
                consulta = consulta.Where(c => c.NombreCristaleria.Contains(buscar) || 
                                              c.CapacidadOz.ToString().Contains(buscar));
            }

            // 2. ORDENACIÓN
            switch (orden)
            {
                case "nombre_desc":
                    consulta = consulta.OrderByDescending(c => c.NombreCristaleria);
                    break;
                case "capacidad_asc":
                    consulta = consulta.OrderBy(c => c.CapacidadOz);
                    break;
                case "capacidad_desc":
                    consulta = consulta.OrderByDescending(c => c.CapacidadOz);
                    break;
                default:
                    consulta = consulta.OrderBy(c => c.NombreCristaleria);
                    break;
            }

            // 3. PAGINACIÓN (Máximo 10 registros)
            int registrosPorPagina = 10;
            int numeroPagina = pagina ?? 1;
            int totalRegistros = await consulta.CountAsync();

            var listado = await consulta
                .Skip((numeroPagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToListAsync();

            ViewData["PaginaActual"] = numeroPagina;
            ViewData["TotalPaginas"] = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);
            ViewData["TotalRegistros"] = totalRegistros;
            ViewData["RegistroInicio"] = totalRegistros == 0 ? 0 : (numeroPagina - 1) * registrosPorPagina + 1;
            ViewData["RegistroFin"] = Math.Min(numeroPagina * registrosPorPagina, totalRegistros);

            return View(listado);
        }

        // GET: Cristaleria/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var cristalerium = await _context.Cristaleria
                .FirstOrDefaultAsync(m => m.IdCristaleria == id);
            if (cristalerium == null) return NotFound();

            return View(cristalerium);
        }

        // GET: Cristaleria/Create
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cristaleria/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create([Bind("IdCristaleria,NombreCristaleria,CapacidadOz")] Cristalerium cristalerium)
        {
            ModelState.Remove("Coctels");

            if (ModelState.IsValid)
            {
                _context.Add(cristalerium);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cristalerium);
        }

        // GET: Cristaleria/Edit/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var cristalerium = await _context.Cristaleria.FindAsync(id);
            if (cristalerium == null) return NotFound();

            return View(cristalerium);
        }

        // POST: Cristaleria/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id, [Bind("IdCristaleria,NombreCristaleria,CapacidadOz")] Cristalerium cristalerium)
        {
            if (id != cristalerium.IdCristaleria) return NotFound();

            ModelState.Remove("Coctels");

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
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int id)
        {
            var cristaleria = await _context.Cristaleria.FindAsync(id);
            if (cristaleria != null)
            {
                bool vasoEnUso = _context.Coctels.Any(c => c.IdCristaleria == id);
                if (vasoEnUso)
                {
                    TempData["Error"] = $"No se puede eliminar la cristalería '{cristaleria.NombreCristaleria}' porque está asociada a uno o más cócteles.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Cristaleria.Remove(cristaleria);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Cristalería eliminada con éxito.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CristaleriumExists(int id)
        {
            return _context.Cristaleria.Any(e => e.IdCristaleria == id);
        }
    }
}
