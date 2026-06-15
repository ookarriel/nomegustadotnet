using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaManejoBar.Models;

public partial class Coctel
{
    [Key]
    public int IdCoctel { get; set; }
    [Required]
    [StringLength(50)]
    public string NombreCoctel { get; set; } = null!;
    
    [Required]
    [StringLength(5000)]
    public string Instrucciones { get; set; } = null!;
    [Required]
    public int PrecioVenta { get; set; }
    [StringLength(255)]
    public string? Foto { get; set; }
   
    public int IdCategoria { get; set; }
    
    public int IdCristaleria { get; set; }

    public virtual ICollection<DetalleRecetum> DetalleReceta { get; set; } = new List<DetalleRecetum>();

    public virtual CategoriaCoctel IdCategoriaNavigation { get; set; } = null!;

    public virtual Cristalerium IdCristaleriaNavigation { get; set; } = null!;
}
