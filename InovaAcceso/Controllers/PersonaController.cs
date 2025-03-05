using Microsoft.AspNetCore.Mvc;
using InovaAcceso.Data;
using InovaAcceso.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using InovaAcceso.Service;
using System.Security.Claims;
using InovaAcceso.Filters;


namespace InovaAcceso.Controllers
{

    public class PersonaController : Controller
    {
        private readonly AppDBContext _appDbContext;
        private readonly UsuarioService _usuarioService;
        public PersonaController(AppDBContext appDbContext, UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
            _appDbContext = appDbContext;
            if (true)
            {
            }
        }

        [AuthorizeSession("Admin")]
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
        [AuthorizeSession("Admin")]
        [HttpGet]
        public IActionResult AgregarPersona()
        {
            CargarListasDeSeleccion();
            return View();
        }
        [AuthorizeSession("Admin")]
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
                Persona personaBusqueda = await _appDbContext.Personas
                    .Include(p => p.Cargo)
                    .Include(p => p.Estado)
                    .Include(p => p.TipoDocumento)
                                .Include(p => p.Rol)
                    .FirstOrDefaultAsync(p => p.NumeroDocumento == persona.NumeroDocumento);

                if (personaBusqueda != null)
                {
                    CargarListasDeSeleccion();
                    TempData["ErrorMessage"] = "La persona ya se encuentra registrada.";
                    return View(persona);
                }
                else
                {
                    string passwordInput = $"{persona.PrimerApellido}{persona.NumeroDocumento}";
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(passwordInput);
                    persona.Contrasena = hashedPassword;

                    persona.Restablecer = true;
                    persona.FechaCreacion = DateTime.Now;
                    persona.FechaModificacion = DateTime.Now;

                    await _appDbContext.Personas.AddAsync(persona);
                    await _appDbContext.SaveChangesAsync();

                    EmailSettings email = new EmailSettings()
                    {
                        To = persona.Email,
                        Subject = "Tus credenciales de acceso",
                        Body = $"Hola {persona.PrimerNombre},<br/><br/>Tu cuenta ha sido creada con éxito.<br/>Número de Documento: {persona.NumeroDocumento}<br/>Contraseña: {passwordInput}<br/><br/>Saludos.<br/>Tu equipo"
                    };

                    bool enviado = CorreoServicio.Enviar(email);

                    if (enviado)
                    {
                        CargarListasDeSeleccion();
                        TempData["SuccessMessage"] = "Persona agregada exitosamente.";
                        return RedirectToAction(nameof(ListaPersonas));
                    }
                    else
                    {
                        CargarListasDeSeleccion();
                        TempData["ErrorMessage"] = "NO se envio correo";
                        return View(persona);

                    }
                }

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "No se pudo guardar la persona. Inténtalo de nuevo. " + ex.InnerException?.Message + ex.Message;
                CargarListasDeSeleccion();
                return View(persona);
            }
        }
        [AuthorizeSession("Admin")]
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
        [AuthorizeSession("Admin")]
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
                personaExistente.ResponsableModificacion = _usuarioService.UsuarioNombres; // Asignar el usuario actual

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
        [AuthorizeSession("Admin")]
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
        [AuthorizeSession("Admin")]
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
        [AuthorizeSession("User")]
        public async Task<IActionResult> InformacionPersonal()
        {
            // Obtener el email del usuario autenticado
            string usuarioEmail = _usuarioService.UsuarioEmail;

            if (string.IsNullOrEmpty(usuarioEmail))
            {
                TempData["ErrorMessage"] = "No se pudo obtener el correo electrónico del usuario.";
                return RedirectToAction("Index", "Home"); // Redirigir si no se encuentra el email
            }

            // Buscar la persona en la base de datos
            var persona = await _appDbContext.Personas
                .Include(p => p.TipoDocumento)
                .Include(p => p.Cargo)
                .Include(p => p.Estado)
                .Include(p => p.Rol)
                .FirstOrDefaultAsync(p => p.Email == usuarioEmail);

            if (persona == null)
            {
                TempData["ErrorMessage"] = "No se encontró información asociada al usuario.";
                return RedirectToAction("Index", "Home"); // Redirigir si no se encuentra la persona
            }

            // Cargar listas de selección para la vista
            CargarListasDeSeleccion();
            return View(persona);
        }
        [HttpPost]
        [ValidateAntiForgeryToken] // Protección contra ataques CSRF
        [AuthorizeSession("User")]
        public async Task<IActionResult> ActualizarInfo(Persona persona)
        {
            if (persona == null)
            {
                TempData["ErrorMessage"] = "Los datos enviados no son válidos.";
                return RedirectToAction(nameof(InformacionPersonal));
            }

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Por favor, revisa los datos ingresados.";
                CargarListasDeSeleccion();
                return View("InformacionPersonal", persona); // Retornar la misma vista con los errores
            }

            try
            {
                // Obtener el email del usuario autenticado
                string usuarioEmail = _usuarioService.UsuarioEmail;

                // Buscar la persona existente en la base de datos
                var personaExistente = await _appDbContext.Personas
                    .FirstOrDefaultAsync(p => p.Email == usuarioEmail);

                if (personaExistente == null)
                {
                    TempData["ErrorMessage"] = "No se encontró información asociada al usuario.";
                    return RedirectToAction(nameof(InformacionPersonal));
                }

                // Actualizar solo los campos editables
                personaExistente.Email = persona.Email;
                personaExistente.Telefono = persona.Telefono;
                personaExistente.Direccion = persona.Direccion;

                // Guardar cambios en la base de datos
                await _appDbContext.SaveChangesAsync();

                TempData["SuccessMessage"] = "Información actualizada exitosamente.";
                return RedirectToAction(nameof(InformacionPersonal));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ocurrió un error al intentar actualizar la información: {ex.Message}";
                return RedirectToAction(nameof(InformacionPersonal));
            }
        }
        // Método para cargar listas de selección
        private void CargarListasDeSeleccion()
        {
            ViewBag.Cargos = new SelectList(_appDbContext.Cargos, "IdCargo", "NombreCargo");
            ViewBag.Estados = new SelectList(_appDbContext.Estados, "IdEstado", "NombreEstado");
            ViewBag.TipoDocs = new SelectList(_appDbContext.TipoDocumentos, "IdTipoDoc", "Documento");
            ViewBag.Roles = new SelectList(_appDbContext.Rols, "IdRol", "NombreRol");
        }
    }
}