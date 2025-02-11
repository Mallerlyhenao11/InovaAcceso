using Microsoft.AspNetCore.Mvc;
using InovaAcceso.Data;
using InovaAcceso.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using InovaAcceso.Service;
using System.Threading.Tasks;
using System.Security.Claims;

namespace InovaAcceso.Controllers
{
    [Authorize]
    public class PersonaController : Controller
    {
        private readonly AppDBContext _appDbContext;

        public PersonaController(AppDBContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> ListaPersonas(string searchString, int? pageNumber)
        {
            var personas = _appDbContext.Personas
                .Include(p => p.Cargo)
                .Include(p => p.Estado)
                .Include(p => p.TipoDocumento)
                .Include(p => p.Rol)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                personas = personas.Where(s => s.PrimerNombre.Contains(searchString)
                    || s.SegundoNombre.Contains(searchString)
                    || s.PrimerApellido.Contains(searchString)
                    || s.SegundoApellido.Contains(searchString)
                    || s.NumeroDocumento.ToString().Contains(searchString));
            }

            int pageSize = 10;
            return View(await PaginatedList<Persona>.CreateAsync(personas.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        [HttpGet]
        public IActionResult AgregarPersona()
        {
            CargarListasDeSeleccion();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AgregarPersona(Persona persona)
        {
            if (!ModelState.IsValid)
            {
                CargarListasDeSeleccion();
                TempData["ErrorMessage"] = "Por favor, revisa los datos ingresados.";
                return View(persona);
            }

            try
            {
                string passwordInput = $"{persona.PrimerApellido}{persona.NumeroDocumento}";
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(passwordInput);
                persona.Contrasena = System.Text.Encoding.UTF8.GetBytes(hashedPassword);
                persona.Restablecer = true;
                persona.FechaCreacion = DateTime.Now;
                persona.FechaModificacion = DateTime.Now;

                await _appDbContext.Personas.AddAsync(persona);
                await _appDbContext.SaveChangesAsync();
                
                /*
                var emailSender = HttpContext.RequestServices.GetRequiredService<IEmailSender>();
                string subject = "Tus credenciales de acceso";
                string body = $"Hola {persona.PrimerNombre},<br/><br/>Tu cuenta ha sido creada con éxito.<br/>Número de Documento: {persona.NumeroDocumento}<br/>Contraseña: {passwordInput}<br/><br/>Saludos,<br/>Tu equipo";
                await emailSender.SendEmailAsync(persona.Email, subject, body);
                */
                TempData["SuccessMessage"] = "Persona agregada exitosamente.";
                return RedirectToAction(nameof(ListaPersonas));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "No se pudo guardar la persona. Inténtalo de nuevo. " + ex.Message;
                CargarListasDeSeleccion();
                return View(persona);
            }
        }

        public async Task<IActionResult> EditarPersona(int id)
        {
            Persona persona = await _appDbContext.Personas
                .Include(p => p.Cargo)
                .Include(p => p.Estado)
                .Include(p => p.TipoDocumento)
                .Include(p => p.Rol)
                .FirstOrDefaultAsync(p => p.IdPersona == id);

            if (persona == null)
            {
                TempData["ErrorMessage"] = "La persona no fue encontrada.";
               return RedirectToAction(nameof(ListaPersonas));
            }

            CargarListasDeSeleccion();
            return View(persona);
        }

        [HttpPost]
        public async Task<IActionResult> EditarPersona(Persona persona)
        {


            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Por favor, revisa los datos ingresados.";
                CargarListasDeSeleccion();
                return View(persona);
            }

            try
            {
                var personaExistente = await _appDbContext.Personas
                    .Include(p => p.Cargo)
                    .Include(p => p.Estado)
                    .Include(p => p.Rol)
                    .Include(p => p.TipoDocumento)
                    .FirstOrDefaultAsync(p => p.IdPersona == persona.IdPersona);

                if (personaExistente == null)
                {
                    TempData["ErrorMessage"] = "La persona no fue encontrada.";
                    return RedirectToAction(nameof(ListaPersonas));
                }

                // Actualizar los datos de la persona
                personaExistente.NumeroDocumento = persona.NumeroDocumento;
                personaExistente.PrimerNombre = persona.PrimerNombre;
                personaExistente.SegundoNombre = persona.SegundoNombre;
                personaExistente.PrimerApellido = persona.PrimerApellido;
                personaExistente.SegundoApellido = persona.SegundoApellido;
                personaExistente.FechaNacimiento = persona.FechaNacimiento;
                personaExistente.Edad = persona.Edad;
                personaExistente.Sexo = persona.Sexo;
                personaExistente.FechaIngreso = persona.FechaIngreso;
                personaExistente.Direccion = persona.Direccion;
                personaExistente.Telefono = persona.Telefono;
                personaExistente.Email = persona.Email;
                personaExistente.IdCargo = persona.IdCargo;
                personaExistente.IdEstado = persona.IdEstado;
                personaExistente.IdTipoDoc = persona.IdTipoDoc;
                personaExistente.IdRol = persona.IdRol;
                personaExistente.FechaModificacion = DateTime.Now;
                personaExistente.ResponsableModificacion = User.Identity.Name; // Asignar el usuario actual

                _appDbContext.Personas.Update(personaExistente);
                await _appDbContext.SaveChangesAsync();

                TempData["SuccessMessage"] = "Persona editada exitosamente.";
                return RedirectToAction(nameof(ListaPersonas));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ocurrió un error al intentar editar a la persona: " + ex.Message;
                CargarListasDeSeleccion();
                return View(persona);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EliminarPersona(int id)
        {
            Persona persona = await _appDbContext.Personas
                .Include(p => p.Cargo)
                .Include(p => p.Estado)
                .Include(p => p.TipoDocumento)
                .Include(p => p.Rol)
                .FirstOrDefaultAsync(p => p.IdPersona == id);

            if (persona == null)
            {
                TempData["ErrorMessage"] = "La persona no fue encontrada.";
                return RedirectToAction(nameof(ListaPersonas));
            }

            return View(persona);
        }

        [HttpPost, ActionName("EliminarPersona")]
        public async Task<IActionResult> ConfirmarEliminarPersona(int id)
        {
            try
            {
                Persona persona = await _appDbContext.Personas.FindAsync(id);

                if (persona == null)
                {
                    TempData["ErrorMessage"] = "La persona no fue encontrada.";
                    return RedirectToAction(nameof(ListaPersonas));
                }

                _appDbContext.Personas.Remove(persona);
                await _appDbContext.SaveChangesAsync();

                TempData["SuccessMessage"] = "Persona eliminada exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ocurrió un error al intentar eliminar a la persona: " + ex.Message;
            }

            return RedirectToAction(nameof(ListaPersonas));
        }

        private void CargarListasDeSeleccion()
        {
            ViewBag.Cargos = new SelectList(_appDbContext.Cargos, "IdCargo", "NombreCargo");
            ViewBag.Estados = new SelectList(_appDbContext.Estados, "IdEstado", "NombreEstado");
            ViewBag.TipoDocs = new SelectList(_appDbContext.TipoDocumentos, "IdTipoDoc", "Documento");
            ViewBag.Roles = new SelectList(_appDbContext.Rols, "IdRol", "NombreRol");
        }
    }
}
