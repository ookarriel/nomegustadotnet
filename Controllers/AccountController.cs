using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SistemaManejoBar.Models;

namespace SistemaManejoBar.Controllers
{
    // Controlador para gestionar el registro, inicio y cierre de sesión de usuarios
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        // GET: Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Verificar si el correo ya existe
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "El correo electrónico ya se encuentra registrado.");
                    return View(model);
                }

                // Crear instancia del usuario personalizado
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    NombreCompleto = model.NombreCompleto,
                    FechaRegistro = DateTime.Now,
                    Activo = true
                };

                // Guardar en base de datos
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Asignar rol "Usuario" por defecto
                    if (await _roleManager.RoleExistsAsync("Usuario"))
                    {
                        await _userManager.AddToRoleAsync(user, "Usuario");
                    }

                    // Iniciar sesión automáticamente
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                // Agregar errores de validacion de Identity (ej: contraseña debil)
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        // GET: Account/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    // Validar si el usuario está activo
                    if (!user.Activo)
                    {
                        ModelState.AddModelError(string.Empty, "Su cuenta se encuentra inactiva. Comuníquese con el administrador.");
                        return View(model);
                    }

                    // Intentar iniciar sesión con bloqueo habilitado (5 fallos -> 5 min bloqueo)
                    var result = await _signInManager.PasswordSignInAsync(
                        user.UserName!, 
                        model.Password, 
                        model.RememberMe, 
                        lockoutOnFailure: true);

                    if (result.Succeeded)
                    {
                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }

                        // Redireccion por rol por defecto
                        if (await _userManager.IsInRoleAsync(user, "Administrador"))
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            return RedirectToAction("Index", "Coctels");
                        }
                    }

                    if (result.IsLockedOut)
                    {
                        ModelState.AddModelError(string.Empty, "Cuenta bloqueada temporalmente por exceso de intentos fallidos. Intente de nuevo en 5 minutos.");
                        return View(model);
                    }
                }

                ModelState.AddModelError(string.Empty, "Credenciales incorrectas o cuenta no existente.");
            }

            return View(model);
        }

        // POST: Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}
