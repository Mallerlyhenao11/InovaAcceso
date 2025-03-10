using InovaAcceso.Data;
using InovaAcceso.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InovaAcceso.Service;

namespace InovaAcceso.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDBContext _appDbContext;
        private readonly UsuarioService _usuarioService;
        public HomeController(ILogger<HomeController> logger, AppDBContext appDbContext, UsuarioService usuarioService)
        {
            _logger = logger;
            _appDbContext = appDbContext;
            _usuarioService = usuarioService;
        }
        public IActionResult Privacy()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(Login login)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Mensaje"] = "Por favor, complete todos los campos correctamente.";
                return View(login);
            }

            var persona_encontrada = await _appDbContext.Personas
                .FirstOrDefaultAsync(u => u.Email == login.Email);

            if (persona_encontrada == null)
            {
                ViewData["Mensaje"] = "No se encontró el usuario.";
                return View(login);
            }

            if (string.IsNullOrEmpty(persona_encontrada.Contrasena) ||
                !BCrypt.Net.BCrypt.Verify(login.Clave, persona_encontrada.Contrasena))
            {
                ViewData["Mensaje"] = "Credenciales incorrectas.";
                return View(login);
            }

            var rol = await _appDbContext.Rols
                .Where(r => r.IdRol == persona_encontrada.IdRol)
                .Select(r => r.NombreRol)
                .FirstOrDefaultAsync();

            // Guardar datos en Session
                HttpContext.Session.SetString("UsuarioRol", rol ?? "Usuario");
                _usuarioService.UsuarioEmail = persona_encontrada.Email;
                _usuarioService.UsuarioNombres = $"{persona_encontrada.PrimerNombre} {persona_encontrada.PrimerApellido}";
                _usuarioService.UsuarioRol = rol;
                _usuarioService.UsuarioId = persona_encontrada.IdPersona;

            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult Reloj()
        {
            return View("Reloj");
        }
        public async Task<IActionResult> Index()
        {
            DateTime fechaActual = DateTime.Now;
            TimeSpan horaActual = DateTime.Now.TimeOfDay;

            var empleadosEnTurno = await _appDbContext.GestionTurnos
                .Include(gt => gt.Persona)
                .Include(gt => gt.Turno)
                .Where(gt =>
                    fechaActual >= gt.FechaInicio &&
                    fechaActual <= gt.FechaFin &&
                    horaActual >= gt.Turno.HoraIngreso &&
                    horaActual <= gt.Turno.HoraSalida)
                .Select(gt => new
                {
                    NombreCompleto = $"{gt.Persona.PrimerNombre} {gt.Persona.PrimerApellido}",
                    Turno = gt.Turno.NombreTurno
                })
                .ToListAsync();

            ViewData["EmpleadosEnTurno"] = empleadosEnTurno;

            return View();
        }
        [HttpGet]
        public IActionResult Salir()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Reloj", "Home");
        }

       

    }
}
