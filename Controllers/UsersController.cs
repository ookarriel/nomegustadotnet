using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SistemaManejoBar.Models;
using Microsoft.AspNetCore.Authorization;

namespace SistemaManejoBar.Controllers
{
    // Controlador exclusivo para la administración de usuarios del sistema
    [Authorize(Roles = "Administrador")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var usuarios = await _userManager.Users.ToListAsync();
            var listadoVM = new List<UserListViewModel>();

            foreach (var user in usuarios)
            {
                var roles = await _userManager.GetRolesAsync(user);
                listadoVM.Add(new UserListViewModel
                {
                    Id = user.Id,
                    NombreCompleto = user.NombreCompleto,
                    Email = user.Email ?? string.Empty,
                    Rol = roles.FirstOrDefault() ?? "Ninguno",
                    Activo = user.Activo
                });
            }

            var rolesDisponibles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            ViewBag.RolesDisponibles = rolesDisponibles;

            return View(listadoVM);
        }

        // GET: Users/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.RolesDisponibles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Verificar si el correo ya existe
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "El correo electrónico ya se encuentra registrado.");
                    ViewBag.RolesDisponibles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
                    return View(model);
                }

                // Crear el nuevo usuario
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    NombreCompleto = model.NombreCompleto,
                    FechaRegistro = DateTime.Now,
                    Activo = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Asignar el rol seleccionado
                    if (await _roleManager.RoleExistsAsync(model.Rol))
                    {
                        await _userManager.AddToRoleAsync(user, model.Rol);
                    }

                    TempData["Exito"] = $"Usuario '{user.Email}' creado correctamente.";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ViewBag.RolesDisponibles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            return View(model);
        }

        // POST: Users/ToggleActivo/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActivo(string id)
        {
            var usuarioLogueado = await _userManager.GetUserAsync(User);
            if (usuarioLogueado?.Id == id)
            {
                TempData["Error"] = "No puedes desactivar tu propia cuenta de administrador.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.Activo = !user.Activo;
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    TempData["Exito"] = $"Estado de activación de '{user.Email}' actualizado correctamente.";
                }
                else
                {
                    TempData["Error"] = "Error al actualizar el estado de activación.";
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Users/CambiarRol
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarRol(string id, string nuevoRol)
        {
            var usuarioLogueado = await _userManager.GetUserAsync(User);
            if (usuarioLogueado?.Id == id)
            {
                TempData["Error"] = "No puedes cambiar tu propio rol de administrador.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                if (!await _roleManager.RoleExistsAsync(nuevoRol))
                {
                    TempData["Error"] = "El rol seleccionado no existe.";
                    return RedirectToAction(nameof(Index));
                }

                var rolesActuales = await _userManager.GetRolesAsync(user);
                var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesActuales);

                if (removeResult.Succeeded)
                {
                    var addResult = await _userManager.AddToRoleAsync(user, nuevoRol);
                    if (addResult.Succeeded)
                    {
                        TempData["Exito"] = $"Rol de '{user.Email}' cambiado a '{nuevoRol}' correctamente.";
                    }
                    else
                    {
                        TempData["Error"] = "Error al asignar el nuevo rol.";
                    }
                }
                else
                {
                    TempData["Error"] = "Error al remover los roles anteriores.";
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Users/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var usuarioLogueado = await _userManager.GetUserAsync(User);
            if (usuarioLogueado?.Id == id)
            {
                TempData["Error"] = "No puedes eliminar tu propia cuenta de administrador.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    TempData["Exito"] = $"Usuario '{user.Email}' eliminado con éxito.";
                }
                else
                {
                    TempData["Error"] = "Ocurrió un error al eliminar el usuario.";
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
