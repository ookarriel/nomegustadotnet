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
    // Controlador para gestionar los ingredientes del bar
    [Authorize(Roles = "Administrador")]
    public class IngredientesController : Controller
    {
        private readonly BarraDbContext _context;

        public IngredientesController(BarraDbContext context)
        {
            _context = context;
        }

        // GET: Ingredientes (Con Búsqueda, Ordenación y Paginación)
        public async Task<IActionResult> Index(string? buscar, string? orden, int? pagina)
        {
            // Persistir los filtros en la vista
            ViewData["BuscarActual"] = buscar;
            ViewData["OrdenActual"] = orden;

            // Parámetros de ordenación
            ViewData["NombreSortParam"] = string.IsNullOrEmpty(orden) ? "nombre_desc" : "";
            ViewData["StockSortParam"] = orden == "stock_asc" ? "stock_desc" : "stock_asc";
            ViewData["TipoSortParam"] = orden == "tipo_asc" ? "tipo_desc" : "tipo_asc";

            var consulta = _context.Ingredientes
                .Include(i => i.IdTipoIngredienteNavigation)
                .AsQueryable();

            // 1. FILTRO / BUSQUEDA (al menos 2 campos)
            if (!string.IsNullOrEmpty(buscar))
            {
                consulta = consulta.Where(i => i.NombreIngrediente.Contains(buscar) || 
                                              i.Unidad.Contains(buscar) || 
                                              i.IdTipoIngredienteNavigation.NombreTipoIngrediente.Contains(buscar));
            }

            // 2. ORDENACIÓN
            switch (orden)
            {
                case "nombre_desc":
                    consulta = consulta.OrderByDescending(i => i.NombreIngrediente);
                    break;
                case "stock_asc":
                    consulta = consulta.OrderBy(i => i.Stock);
                    break;
                case "stock_desc":
                    consulta = consulta.OrderByDescending(i => i.Stock);
                    break;
                case "tipo_asc":
                    consulta = consulta.OrderBy(i => i.IdTipoIngredienteNavigation.NombreTipoIngrediente);
                    break;
                case "tipo_desc":
                    consulta = consulta.OrderByDescending(i => i.IdTipoIngredienteNavigation.NombreTipoIngrediente);
                    break;
                default:
                    consulta = consulta.OrderBy(i => i.NombreIngrediente);
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

        // GET: Ingredientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var ingrediente = await _context.Ingredientes
                .Include(i => i.IdTipoIngredienteNavigation)
                .FirstOrDefaultAsync(m => m.IdIngrediente == id);
            if (ingrediente == null) return NotFound();

            return View(ingrediente);
        }

        // GET: Ingredientes/Create
        [Authorize(Roles = "Administrador,Supervisor")]
        public IActionResult Create()
        {
            ViewData["IdTipoIngrediente"] = new SelectList(_context.TipoIngredientes, "IdTipoIngrediente", "NombreTipoIngrediente");
            return View();
        }

        // POST: Ingredientes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Supervisor")]
        public async Task<IActionResult> Create([Bind("IdIngrediente,NombreIngrediente,Stock,Unidad,IdTipoIngrediente")] Ingrediente ingrediente)
        {
            // Ignorar validación de las relaciones
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
        [Authorize(Roles = "Administrador,Supervisor")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var ingrediente = await _context.Ingredientes.FindAsync(id);
            if (ingrediente == null) return NotFound();

            ViewData["IdTipoIngrediente"] = new SelectList(_context.TipoIngredientes, "IdTipoIngrediente", "NombreTipoIngrediente", ingrediente.IdTipoIngrediente);
            return View(ingrediente);
        }

        // POST: Ingredientes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Supervisor")]
        public async Task<IActionResult> Edit(int id, [Bind("IdIngrediente,NombreIngrediente,Stock,Unidad,IdTipoIngrediente")] Ingrediente ingrediente)
        {
            if (id != ingrediente.IdIngrediente) return NotFound();

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
        [Authorize(Roles = "Administrador,Supervisor")]
        public async Task<IActionResult> Delete(int id)
        {
            var ingrediente = await _context.Ingredientes.FindAsync(id);
            if (ingrediente != null)
            {
                var detallesAsociados = _context.DetalleReceta.Where(d => d.IdIngrediente == id);
                _context.DetalleReceta.RemoveRange(detallesAsociados);
                _context.Ingredientes.Remove(ingrediente);
                await _context.SaveChangesAsync();
                TempData["Exito"] = "Ingrediente eliminado de la barra.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool IngredienteExists(int id)
        {
            return _context.Ingredientes.Any(e => e.IdIngrediente == id);
        }
    }
}