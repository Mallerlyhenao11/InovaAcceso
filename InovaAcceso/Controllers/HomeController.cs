using InovaAcceso.Data;
using InovaAcceso.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace InovaAcceso.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDBContext _appDbContext;

        public HomeController(ILogger<HomeController> logger, AppDBContext appDbContext)
        {
            _logger = logger;
            _appDbContext = appDbContext;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Login login)
        {
            // Validar el modelo
            if (!ModelState.IsValid)
            {
                ViewData["Mensaje"] = "Por favor, complete todos los campos correctamente.";
                return View(login);
            }

            // Buscar al usuario en la base de datos
            Persona persona_encontrada = await _appDbContext.Personas
                .Where(u => u.Email == login.Email)
                .FirstOrDefaultAsync();

            if (persona_encontrada == null)
            {
                ViewData["Mensaje"] = "No se encontró el usuario.";
                return View(login);
            }

            // Verificar la contraseña
            if (!BCrypt.Net.BCrypt.Verify(login.Clave, persona_encontrada.Contrasena))
            {
                ViewData["Mensaje"] = "Credenciales incorrectas.";
                return View(login);
            }
            var rol = await _appDbContext.Rols
                .Where(r => r.IdRol == persona_encontrada.IdRol)
                .Select(r => r.NombreRol)
                .FirstOrDefaultAsync();
            // Crear los claims (información del usuario autenticado)
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, persona_encontrada.Email),
                new Claim(ClaimTypes.Role, rol), // 
                new Claim(ClaimTypes.Name, persona_encontrada.PrimerNombre + " " + persona_encontrada.PrimerApellido)
            };

            // Crear la identidad del usuario
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Configurar las propiedades de la autenticación
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true, // Permitir que se renueven las cookies
                IsPersistent = login.RememberMe, // Depende de la opción "Recuérdame"
                ExpiresUtc = login.RememberMe ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddHours(1) // 7 días o 1 hora
            };

            // Iniciar la sesión del usuario
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                properties
            );

            // Redirigir al usuario a la página que intentó acceder antes de iniciar sesión (si corresponde)
            if (!string.IsNullOrEmpty(Request.Query["ReturnUrl"]))
            {
                return Redirect(Request.Query["ReturnUrl"]);
            }

            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Obtener la fecha y hora actuales
            DateTime fechaActual = DateTime.Now;
            TimeSpan horaActual = DateTime.Now.TimeOfDay;

            // Consultar los empleados en turno
            var empleadosEnTurno = await _appDbContext.GestionTurnos
                .Include(gt => gt.Persona) // Incluir información de la persona
                .Include(gt => gt.Turno)  // Incluir información del turno
                .Where(gt =>
                    fechaActual >= gt.FechaInicio && // Fecha actual dentro del rango de fechas
                    fechaActual <= gt.FechaFin &&
                    horaActual >= gt.Turno.HoraIngreso && // Hora actual dentro del rango del turno
                    horaActual <= gt.Turno.HoraSalida
                )
                .Select(gt => new
                {
                    NombreCompleto = $"{gt.Persona.PrimerNombre} {gt.Persona.PrimerApellido}",
                    Turno = gt.Turno.NombreTurno
                })
                .ToListAsync();

            // Pasar los datos a la vista
            ViewData["EmpleadosEnTurno"] = empleadosEnTurno;

            return View();
        }

        // Acción para cerrar sesión
        [HttpGet]
        public async Task<IActionResult> Salir()
        {
            // Cierra la sesión del usuario
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Redirige al usuario a la página de inicio de sesión
            return RedirectToAction("Login", "Home");
        }



    }
}