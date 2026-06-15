using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaManejoBar.Models;
using System.IO;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Microsoft.AspNetCore.Authorization;

namespace SistemaManejoBar.Controllers
{
    
    public class CoctelsController : Controller
    {
        private readonly BarraDbContext _context;

        public CoctelsController(BarraDbContext context)
        {
            _context = context;
        }

        // GET: Coctels
        public async Task<IActionResult> Index(int? pagina)
        {
            
            int registrosPorPagina = 5; 
            int numeroPagina = pagina ?? 1; 

            var consulta = _context.Coctels
                .Include(c => c.IdCategoriaNavigation)
                .Include(c => c.IdCristaleriaNavigation);

           
            int totalRegistros = await consulta.CountAsync();

           
            var coctelesPaginados = await consulta
                .Skip((numeroPagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToListAsync();

          
            ViewBag.PaginaActual = numeroPagina;
            ViewBag.TotalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);

            return View(coctelesPaginados);
        }

        // GET: Coctels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

          
            var coctel = await _context.Coctels
                .Include(c => c.IdCategoriaNavigation)
                .Include(c => c.IdCristaleriaNavigation)
                .Include(c => c.DetalleReceta)
                    .ThenInclude(d => d.IdIngredienteNavigation)
                .FirstOrDefaultAsync(m => m.IdCoctel == id);

            if (coctel == null)
            {
                return NotFound();
            }

            return View(coctel);
        }

        // GET: Coctels/Create
        public IActionResult Create()
        {
            ViewData["IdCategoria"] = new SelectList(_context.CategoriaCoctels, "IdCategoria", "NombreCategoria");
            ViewData["IdCristaleria"] = new SelectList(_context.Cristaleria, "IdCristaleria", "NombreCristaleria");
            ViewData["Ingredientes"] = _context.Ingredientes.OrderBy(i => i.NombreIngrediente).ToList();

            return View();
        }

        // POST: Coctels/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCoctel,NombreCoctel,Instrucciones,PrecioVenta,IdCategoria,IdCristaleria")] Coctel coctel, IFormFile? archivo_foto, int[] ingredientesSeleccionados)
        {
            ModelState.Remove("Foto");
            ModelState.Remove("IdCategoriaNavigation");
            ModelState.Remove("IdCristaleriaNavigation");

            if (ModelState.IsValid)
            {
                if (archivo_foto != null && archivo_foto.Length > 0)
                {
                    string carpeta_fotos = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "foto");
                    if (!Directory.Exists(carpeta_fotos))
                    {
                        Directory.CreateDirectory(carpeta_fotos);
                    }

                    string extension = Path.GetExtension(archivo_foto.FileName);
                    string nombreUnicoArchivo = Guid.NewGuid().ToString() + extension;
                    string rutaCompleta = Path.Combine(carpeta_fotos, nombreUnicoArchivo);

                    using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                    {
                        await archivo_foto.CopyToAsync(stream);
                    }

                    coctel.Foto = "/foto/" + nombreUnicoArchivo;
                }
                else
                {
                    coctel.Foto = "/foto/no-imagen.png";
                }

               
                _context.Add(coctel);
                await _context.SaveChangesAsync();

                
                if (ingredientesSeleccionados != null && ingredientesSeleccionados.Length > 0)
                {
                    foreach (var idIngrediente in ingredientesSeleccionados)
                    {
                        var detalle = new DetalleRecetum
                        {
                            IdCoctel = coctel.IdCoctel,
                            IdIngrediente = idIngrediente,
                            Cantidad = 1 
                        };
                        _context.DetalleReceta.Add(detalle);
                    }
                    await _context.SaveChangesAsync(); 
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["IdCategoria"] = new SelectList(_context.CategoriaCoctels, "IdCategoria", "NombreCategoria", coctel.IdCategoria);
            ViewData["IdCristaleria"] = new SelectList(_context.Cristaleria, "IdCristaleria", "NombreCristaleria", coctel.IdCristaleria);
            ViewData["Ingredientes"] = _context.Ingredientes.OrderBy(i => i.NombreIngrediente).ToList();

            return View(coctel);
        }

        // GET: Coctels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

           
            var coctel = await _context.Coctels
                .Include(c => c.DetalleReceta)
                .FirstOrDefaultAsync(c => c.IdCoctel == id);

            if (coctel == null)
            {
                return NotFound();
            }
            ViewData["IdCategoria"] = new SelectList(_context.CategoriaCoctels, "IdCategoria", "NombreCategoria", coctel.IdCategoria);
            ViewData["IdCristaleria"] = new SelectList(_context.Cristaleria, "IdCristaleria", "NombreCristaleria", coctel.IdCristaleria);

          
            ViewData["Ingredientes"] = _context.Ingredientes.OrderBy(i => i.NombreIngrediente).ToList();
            ViewData["IngredientesSeleccionados"] = coctel.DetalleReceta.Select(d => d.IdIngrediente).ToList();

            return View(coctel);
        }

        // POST: Coctels/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
       
        public async Task<IActionResult> Edit(int id, [Bind("IdCoctel,NombreCoctel,Instrucciones,PrecioVenta,Foto,IdCategoria,IdCristaleria")] Coctel coctel, IFormFile? archivo_foto, int[] ingredientesSeleccionados)
        {
            if (id != coctel.IdCoctel)
            {
                return NotFound();
            }
            ModelState.Remove("Foto");
            ModelState.Remove("IdCategoriaNavigation");
            ModelState.Remove("IdCristaleriaNavigation");

            if (ModelState.IsValid)
            {
                try
                {
                    var coctelOriginal = await _context.Coctels.AsNoTracking().FirstOrDefaultAsync(c => c.IdCoctel == id);

                    if (archivo_foto != null && archivo_foto.Length > 0)
                    {
                        string carpeta_fotos = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "foto");
                        if (!Directory.Exists(carpeta_fotos))
                        {
                            Directory.CreateDirectory(carpeta_fotos);
                        }

                        string extension = Path.GetExtension(archivo_foto.FileName);
                        string nombreUnicoArchivo = Guid.NewGuid().ToString() + extension;
                        string rutaCompleta = Path.Combine(carpeta_fotos, nombreUnicoArchivo);

                        using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                        {
                            await archivo_foto.CopyToAsync(stream);
                        }
                        coctel.Foto = "/foto/" + nombreUnicoArchivo;
                    }
                    else
                    {
                        coctel.Foto = coctelOriginal?.Foto;
                    }

                    _context.Update(coctel);

                    var detallesViejos = _context.DetalleReceta.Where(d => d.IdCoctel == id);
                    _context.DetalleReceta.RemoveRange(detallesViejos);

                
                    if (ingredientesSeleccionados != null && ingredientesSeleccionados.Length > 0)
                    {
                        foreach (var idIngrediente in ingredientesSeleccionados)
                        {
                            _context.DetalleReceta.Add(new DetalleRecetum
                            {
                                IdCoctel = coctel.IdCoctel,
                                IdIngrediente = idIngrediente,
                                Cantidad = 1
                            });
                        }
                    }
                

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CoctelExists(coctel.IdCoctel))
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

            ViewData["IdCategoria"] = new SelectList(_context.CategoriaCoctels, "IdCategoria", "NombreCategoria", coctel.IdCategoria);
            ViewData["IdCristaleria"] = new SelectList(_context.Cristaleria, "IdCristaleria", "NombreCristaleria", coctel.IdCristaleria);
            ViewData["Ingredientes"] = _context.Ingredientes.OrderBy(i => i.NombreIngrediente).ToList();

            return View(coctel);
        }

        // POST: Coctels/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var coctel = await _context.Coctels.FindAsync(id);
            if (coctel != null){
            
                var detallesAsociados = _context.DetalleReceta.Where(d => d.IdCoctel == id);
                _context.DetalleReceta.RemoveRange(detallesAsociados);
                _context.Coctels.Remove(coctel);
                await _context.SaveChangesAsync();

            }
            return RedirectToAction(nameof(Index));
        }

        private bool CoctelExists(int id)
        {
            return _context.Coctels.Any(e => e.IdCoctel == id);
        }
        // GET: Coctels/ExportarExcel
        public async Task<IActionResult> ExportarExcel()
        {
            var cocteles = await _context.Coctels
                .Include(c => c.IdCategoriaNavigation)
                .Include(c => c.IdCristaleriaNavigation)
                .ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Cócteles");
                var currentRow = 1;

                
                worksheet.Cell(currentRow, 1).Value = "ID";
                worksheet.Cell(currentRow, 2).Value = "Nombre del Cóctel";
                worksheet.Cell(currentRow, 3).Value = "Categoría";
                worksheet.Cell(currentRow, 4).Value = "Cristalería";
                worksheet.Cell(currentRow, 5).Value = "Precio de Venta";

                
                var headerRange = worksheet.Range("A1:E1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                
                foreach (var item in cocteles)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = item.IdCoctel;
                    worksheet.Cell(currentRow, 2).Value = item.NombreCoctel;
                    worksheet.Cell(currentRow, 3).Value = item.IdCategoriaNavigation?.NombreCategoria ?? "Sin Categoría";
                    worksheet.Cell(currentRow, 4).Value = item.IdCristaleriaNavigation?.NombreCristaleria ?? "Sin Cristalería";

                    
                    worksheet.Cell(currentRow, 5).Value = item.PrecioVenta;
                    worksheet.Cell(currentRow, 5).Style.NumberFormat.Format = "$ #,##0";
                }

                
                worksheet.Columns().AdjustToContents();

                
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reporte_Cocteles.xlsx");
                }
            }
        }
        // GET: Coctels/ExportarPDF
        public async Task<IActionResult> ExportarPDF()
        {
            
            QuestPDF.Settings.License = LicenseType.Community;

            var cocteles = await _context.Coctels
                .Include(c => c.IdCategoriaNavigation)
                .Include(c => c.IdCristaleriaNavigation)
                .ToListAsync();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header().Text($"Carta de cocteles ({DateTime.Now:dd/MM/yyyy})")
                        .SemiBold().FontSize(20).FontColor(Colors.Blue.Darken2);

                   
                    page.Content().PaddingVertical(1, Unit.Centimetre).Table(table =>
                    {
                        
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(30); 
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

            
                        table.Header(header =>
                        {
                            header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).Text("ID").SemiBold();
                            header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).Text("Nombre").SemiBold();
                            header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).Text("Categoría").SemiBold();
                            header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).Text("Cristalería").SemiBold();
                            header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).Text("Precio").SemiBold();
                        });

                        
                        foreach (var item in cocteles)
                        {
                            table.Cell().PaddingVertical(5).Text(item.IdCoctel.ToString());
                            table.Cell().PaddingVertical(5).Text(item.NombreCoctel);
                            table.Cell().PaddingVertical(5).Text(item.IdCategoriaNavigation?.NombreCategoria ?? "N/A");
                            table.Cell().PaddingVertical(5).Text(item.IdCristaleriaNavigation?.NombreCristaleria ?? "N/A");
                            table.Cell().PaddingVertical(5).Text($"${item.PrecioVenta}");
                        }
                    });

                   
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                    });
                });
            });

          
            var pdfBytes = document.GeneratePdf();
            return File(pdfBytes, "application/pdf", "Reporte_Cocteles.pdf");
        }
    }
}
