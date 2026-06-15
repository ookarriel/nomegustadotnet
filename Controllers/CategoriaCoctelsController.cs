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
    // Controlador para gestionar las categorías de cócteles
    [Authorize(Roles = "Administrador")]
    public class CategoriaCoctelsController : Controller
    {
        private readonly BarraDbContext _context;

        public CategoriaCoctelsController(BarraDbContext context)
        {
            _context = context;
        }

        // GET: CategoriaCoctels (Con Búsqueda, Ordenación y Paginación)
        public async Task<IActionResult> Index(string? buscar, string? orden, int? pagina)
        {
            // Persistir los filtros en la vista
            ViewData["BuscarActual"] = buscar;
            ViewData["OrdenActual"] = orden;

            // Parámetros de ordenación alternantes
            ViewData["NombreSortParam"] = string.IsNullOrEmpty(orden) ? "nombre_desc" : "";
            ViewData["DescSortParam"] = orden == "desc_asc" ? "desc_desc" : "desc_asc";

            var consulta = _context.CategoriaCoctels.AsQueryable();

            // 1. FILTRO / BÚSQUEDA
            if (!string.IsNullOrEmpty(buscar))
            {
                consulta = consulta.Where(c => c.NombreCategoria.Contains(buscar) || 
                                              (c.Descripcion != null && c.Descripcion.Contains(buscar)));
            }

            // 2. ORDENACIÓN
            switch (orden)
            {
                case "nombre_desc":
                    consulta = consulta.OrderByDescending(c => c.NombreCategoria);
                    break;
                case "desc_asc":
                    consulta = consulta.OrderBy(c => c.Descripcion);
                    break;
                case "desc_desc":
                    consulta = consulta.OrderByDescending(c => c.Descripcion);
                    break;
                default:
                    consulta = consulta.OrderBy(c => c.NombreCategoria);
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

            // Resumen e info para controles de paginación
            ViewData["PaginaActual"] = numeroPagina;
            ViewData["TotalPaginas"] = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);
            ViewData["TotalRegistros"] = totalRegistros;
            ViewData["RegistroInicio"] = totalRegistros == 0 ? 0 : (numeroPagina - 1) * registrosPorPagina + 1;
            ViewData["RegistroFin"] = Math.Min(numeroPagina * registrosPorPagina, totalRegistros);

            return View(listado);
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
        [Authorize(Roles = "Administrador,Supervisor")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: CategoriaCoctels/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Supervisor")]
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
        [Authorize(Roles = "Administrador,Supervisor")]
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
        [Authorize(Roles = "Administrador,Supervisor")]
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
        [Authorize(Roles = "Administrador,Supervisor")]
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