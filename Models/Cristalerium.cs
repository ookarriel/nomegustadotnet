using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SistemaManejoBar.Models;

public partial class Cristalerium
{
    [Key]
    public int IdCristaleria { get; set; }
    [Required]
    [StringLength(50)]
    public string NombreCristaleria { get; set; } = null!;
    [Required]
    public double CapacidadOz { get; set; }

    public virtual ICollection<Coctel> Coctels { get; set; } = new List<Coctel>();
}
