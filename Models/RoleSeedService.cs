using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SistemaManejoBar.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaManejoBar.Services
{
    // Servicio para la inicialización y limpieza de roles y usuarios
    public static class RoleSeedService
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // 1. ROLES
            string[] roleNames = { "Administrador", "Bartender", "Usuario" };

            // Asegurar que existan los roles correctos
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Eliminar roles antiguos no requeridos (ej: Supervisor, Admin, Mesero)
            var rolesExistentes = await roleManager.Roles.ToListAsync();
            foreach (var rol in rolesExistentes)
            {
                if (rol.Name != null && !roleNames.Contains(rol.Name))
                {
                    await roleManager.DeleteAsync(rol);
                }
            }

            // 2. Crear el Usuario Administrador por defecto
            var adminEmail = "admin@sistema.cl";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var newAdmin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    NombreCompleto = "Administrador del Sistema",
                    FechaRegistro = DateTime.Now,
                    Activo = true
                };

                var createAdmin = await userManager.CreateAsync(newAdmin, "Admin123!");

                if (createAdmin.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Administrador");
                }
            }
            else
            {
                if (!await userManager.IsInRoleAsync(adminUser, "Administrador"))
                {
                    await userManager.AddToRoleAsync(adminUser, "Administrador");
                }
            }
        }
    }
}