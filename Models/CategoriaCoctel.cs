using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaManejoBar.Models;

public partial class CategoriaCoctel
{
    [Key]
    public int IdCategoria { get; set; }
    [Required]
    [StringLength(50)]
    public string NombreCategoria { get; set; } = null!;
    
    public string? Descripcion { get; set; }

    public virtual ICollection<Coctel> Coctels { get; set; } = new List<Coctel>();
}
