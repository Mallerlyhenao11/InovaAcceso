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
            Persona persona_encontrada = await _appDbContext.Personas
                .Where(u => u.Email == login.email)
                .FirstOrDefaultAsync();

            if (persona_encontrada == null)
            {
                ViewData["Mensaje"] = "No se encontró el usuario";
                return View();
            }

            if (BCrypt.Net.BCrypt.Verify(login.clave, System.Text.Encoding.UTF8.GetString(persona_encontrada.Contrasena)))
            {
                Console.WriteLine("La contraseña es válida");
            }
            else
            {
                ViewData["Mensaje"] = "Credenciales incorrectas";
                return View();
            }

            List<Claim> claim = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, persona_encontrada.Email),
                new Claim(ClaimTypes.Name, persona_encontrada.PrimerNombre + " " + persona_encontrada.PrimerApellido)
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claim, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                properties
            );

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

        [HttpGet]
        public async Task<IActionResult> Salir()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Home");
        }
    }
}
