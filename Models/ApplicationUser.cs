using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace SistemaManejoBar.Models
{
    // Clase de usuario que extiende IdentityUser con propiedades personalizadas
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        [Required]
        public bool Activo { get; set; } = true;
    }
}
