using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaManejoBar.Models;

public partial class DetalleRecetum
{
    [Key]
    public int IdDetalleReceta { get; set; }
    
    public int IdCoctel { get; set; }
    
    public int IdIngrediente { get; set; }
    [Required]
    public double Cantidad { get; set; }

    public virtual Coctel IdCoctelNavigation { get; set; } = null!;

    public virtual Ingrediente IdIngredienteNavigation { get; set; } = null!;
}
