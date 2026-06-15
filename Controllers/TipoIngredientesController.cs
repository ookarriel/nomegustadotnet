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
    // Controlador para gestionar los tipos de ingredientes de la barra
    [Authorize(Roles = "Administrador,Bartender")]
    public class TipoIngredientesController : Controller
    {
        private readonly BarraDbContext _context;

        public TipoIngredientesController(BarraDbContext context)
        {
            _context = context;
        }

        // GET: TipoIngredientes (Con Búsqueda, Ordenación y Paginación)
        public async Task<IActionResult> Index(string? buscar, string? orden, int? pagina)
        {
            // Persistir los filtros en la vista
            ViewData["BuscarActual"] = buscar;
            ViewData["OrdenActual"] = orden;

            // Parámetros de ordenación
            ViewData["NombreSortParam"] = string.IsNullOrEmpty(orden) ? "nombre_desc" : "";

            var consulta = _context.TipoIngredientes.AsQueryable();

            // 1. FILTRO / BUSQUEDA (al menos 2 campos en otros, aquí es por nombre o ID)
            if (!string.IsNullOrEmpty(buscar))
            {
                consulta = consulta.Where(t => t.NombreTipoIngrediente.Contains(buscar) || 
                                              t.IdTipoIngrediente.ToString().Contains(buscar));
            }

            // 2. ORDENACIÓN
            if (orden == "nombre_desc")
            {
                consulta = consulta.OrderByDescending(t => t.NombreTipoIngrediente);
            }
            else
            {
                consulta = consulta.OrderBy(t => t.NombreTipoIngrediente);
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

        // GET: TipoIngredientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var tipoIngrediente = await _context.TipoIngredientes
                .FirstOrDefaultAsync(m => m.IdTipoIngrediente == id);
            if (tipoIngrediente == null) return NotFound();

            return View(tipoIngrediente);
        }

        // GET: TipoIngredientes/Create
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: TipoIngredientes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create([Bind("IdTipoIngrediente,NombreTipoIngrediente")] TipoIngrediente tipoIngrediente)
        {
            ModelState.Remove("Ingredientes");

            if (ModelState.IsValid)
            {
                _context.Add(tipoIngrediente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tipoIngrediente);
        }

        // GET: TipoIngredientes/Edit/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var tipoIngrediente = await _context.TipoIngredientes.FindAsync(id);
            if (tipoIngrediente == null) return NotFound();

            return View(tipoIngrediente);
        }

        // POST: TipoIngredientes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id, [Bind("IdTipoIngrediente,NombreTipoIngrediente")] TipoIngrediente tipoIngrediente)
        {
            if (id != tipoIngrediente.IdTipoIngrediente) return NotFound();

            ModelState.Remove("Ingredientes");

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
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var tipoIngrediente = await _context.TipoIngredientes
                .FirstOrDefaultAsync(m => m.IdTipoIngrediente == id);
            if (tipoIngrediente == null) return NotFound();

            return View(tipoIngrediente);
        }

        // POST: TipoIngredientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tipoIngrediente = await _context.TipoIngredientes.FindAsync(id);
            if (tipoIngrediente != null)
            {
                bool enUso = _context.Ingredientes.Any(i => i.IdTipoIngrediente == id);
                if (enUso)
                {
                    TempData["Error"] = $"No se puede eliminar el tipo de ingrediente '{tipoIngrediente.NombreTipoIngrediente}' porque está asociado a uno o más ingredientes.";
                    return RedirectToAction(nameof(Index));
                }

                _context.TipoIngredientes.Remove(tipoIngrediente);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Tipo de ingrediente eliminado correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TipoIngredienteExists(int id)
        {
            return _context.TipoIngredientes.Any(e => e.IdTipoIngrediente == id);
        }
    }
}
