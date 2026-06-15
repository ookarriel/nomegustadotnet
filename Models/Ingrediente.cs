using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaManejoBar.Models;

public partial class Ingrediente
{
    [Key]
    public int IdIngrediente { get; set; }
    [Required]
    [StringLength(50)]
    public string NombreIngrediente { get; set; } = null!;
    [Required]
    public double Stock { get; set; }
    [Required]
    public string Unidad { get; set; } = null!;

    public int IdTipoIngrediente { get; set; }

    public virtual ICollection<DetalleRecetum> DetalleReceta { get; set; } = new List<DetalleRecetum>();

    public virtual TipoIngrediente IdTipoIngredienteNavigation { get; set; } = null!;
}
