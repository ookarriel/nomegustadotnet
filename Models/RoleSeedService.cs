using Microsoft.AspNetCore.Identity;

namespace SistemaManejoBar.Services;

public static class RoleSeedService
{
    public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
    {
        // Gestores de usuarios
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        // 1. ROLES
        string[] roleNames = { "Admin", "Bartender", "Mesero" };

        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                // Crea el rol si no existe e
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // 2. Crear el Usuario Administrador 
        var adminEmail = "admin@barramanejo.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            var newAdmin = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            // Creamos el usuario con una contraseña por defecto segura
            var createAdmin = await userManager.CreateAsync(newAdmin, "Admin123!");

            if (createAdmin.Succeeded)
            {
                // Asignamos el rol de Admin al nuevo usuario
                await userManager.AddToRoleAsync(newAdmin, "Admin");
            }
        }
    }
}