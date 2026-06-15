using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaManejoBar.Models;

public partial class TipoIngrediente
{
    [Key]
    public int IdTipoIngrediente { get; set; }
    [Required]
    [StringLength(50)]
    public string NombreTipoIngrediente { get; set; } = null!;

    public virtual ICollection<Ingrediente> Ingredientes { get; set; } = new List<Ingrediente>();
}
