using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InovaAcceso.Data;
using InovaAcceso.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using InovaAcceso.Filters;
using InovaAcceso.Service;

namespace InovaAcceso.Controllers
{
    public class GestionTurnoController : Controller
    {
        private readonly AppDBContext _appDbContext;
        private readonly UsuarioService _usuarioService;

        public GestionTurnoController(AppDBContext appDbContext, UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
            _appDbContext = appDbContext;
        }

        [AuthorizeSession("Admin")]
        [HttpGet]
        public async Task<IActionResult> listaAsignacionTurno(string searchString, int? pageNumber)
        {
            var gestionTurnos = _appDbContext.GestionTurnos
                .Include(g => g.Persona)
                .Include(g => g.Turno)
                .AsQueryable();

            // Filtro de búsqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                gestionTurnos = gestionTurnos.Where(g =>
                    g.Persona.PrimerNombre.Contains(searchString) ||
                    g.Persona.PrimerApellido.Contains(searchString) ||
                    g.Turno.NombreTurno.Contains(searchString));
            }

            int pageSize = 10;
            var paginatedList = await PaginatedList<GestionTurno>.CreateAsync(gestionTurnos.AsNoTracking(), pageNumber ?? 1, pageSize);
            return View(paginatedList);
        }

        [HttpGet]
        public IActionResult asignarTurno()
        {
            cargarListasDeSeleccion();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> asignarTurno(GestionTurno gestionTurno)
        {
            try
            {
                // Validar que la fecha fin sea posterior a la fecha inicio
                if (gestionTurno.FechaFin <= gestionTurno.FechaInicio)
                {
                    TempData["ErrorMessage"] = "La fecha de fin debe ser posterior a la fecha de inicio.";
                    cargarListasDeSeleccion();
                    return View(gestionTurno);
                }

                // Verificar si ya existe una asignación del MISMO turno para la persona en fechas que se solapan
                var turnoExistente = await _appDbContext.GestionTurnos
                    .Where(gt =>
                        gt.IdPersona == gestionTurno.IdPersona &&
                        gt.IdTurno == gestionTurno.IdTurno && // Mismo turno
                        ((gt.FechaInicio <= gestionTurno.FechaInicio && gt.FechaFin >= gestionTurno.FechaInicio) ||
                         (gt.FechaInicio <= gestionTurno.FechaFin && gt.FechaFin >= gestionTurno.FechaFin) ||
                         (gt.FechaInicio >= gestionTurno.FechaInicio && gt.FechaFin <= gestionTurno.FechaFin)))
                    .FirstOrDefaultAsync();

                if (turnoExistente != null)
                {
                    TempData["ErrorMessage"] = $"La persona ya tiene asignado este turno del {turnoExistente.FechaInicio:dd/MM/yyyy} al {turnoExistente.FechaFin:dd/MM/yyyy}";
                    cargarListasDeSeleccion();
                    return View(gestionTurno);
                }

                // Asignar valores automáticos
                gestionTurno.FechaCreacion = DateTime.Now;
                gestionTurno.FechaModificacion = DateTime.Now;
                gestionTurno.ResponsableModificacion = _usuarioService.UsuarioNombres;

                await _appDbContext.GestionTurnos.AddAsync(gestionTurno);
                await _appDbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Turno asignado exitosamente.";
                return RedirectToAction(nameof(listaAsignacionTurno));
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Ocurrió un error al guardar el turno: " + ex.Message);
                cargarListasDeSeleccion();
                return View(gestionTurno);
            }
        }

        [HttpGet]
        public async Task<IActionResult> editarAsignacionTurno(int id)
        {
            cargarListasDeSeleccion();
            var gestionTurno = await _appDbContext.GestionTurnos.FindAsync(id);
            if (gestionTurno == null)
            {
                return NotFound();
            }
            return View(gestionTurno);
        }

        [HttpPost]
        public async Task<IActionResult> editarAsignacionTurno(GestionTurno gestionTurno)
        {
            if (!ModelState.IsValid)
            {
                cargarListasDeSeleccion();
                return View(gestionTurno);
            }

            try
            {
                // Validar que la fecha fin sea posterior a la fecha inicio
                if (gestionTurno.FechaFin <= gestionTurno.FechaInicio)
                {
                    TempData["ErrorMessage"] = "La fecha de fin debe ser posterior a la fecha de inicio.";
                    cargarListasDeSeleccion();
                    return View(gestionTurno);
                }

                // Verificar si ya existe una asignación del MISMO turno para la persona en fechas que se solapan
                var turnoExistente = await _appDbContext.GestionTurnos
                    .Where(gt =>
                        gt.IdPersona == gestionTurno.IdPersona &&
                        gt.IdTurno == gestionTurno.IdTurno &&
                        gt.IdGestionTurno != gestionTurno.IdGestionTurno && // Excluir el turno actual
                        ((gt.FechaInicio <= gestionTurno.FechaInicio && gt.FechaFin >= gestionTurno.FechaInicio) ||
                         (gt.FechaInicio <= gestionTurno.FechaFin && gt.FechaFin >= gestionTurno.FechaFin) ||
                         (gt.FechaInicio >= gestionTurno.FechaInicio && gt.FechaFin <= gestionTurno.FechaFin)))
                    .FirstOrDefaultAsync();

                if (turnoExistente != null)
                {
                    TempData["ErrorMessage"] = $"La persona ya tiene asignado este turno del {turnoExistente.FechaInicio:dd/MM/yyyy} al {turnoExistente.FechaFin:dd/MM/yyyy}";
                    cargarListasDeSeleccion();
                    return View(gestionTurno);
                }

                var gestionTurnoExistente = await _appDbContext.GestionTurnos
                    .FirstOrDefaultAsync(g => g.IdGestionTurno == gestionTurno.IdGestionTurno);

                if (gestionTurnoExistente != null)
                {
                    gestionTurnoExistente.IdPersona = gestionTurno.IdPersona;
                    gestionTurnoExistente.IdTurno = gestionTurno.IdTurno;
                    gestionTurnoExistente.FechaInicio = gestionTurno.FechaInicio;
                    gestionTurnoExistente.FechaFin = gestionTurno.FechaFin;
                    gestionTurnoExistente.FechaModificacion = DateTime.Now;
                    gestionTurnoExistente.ResponsableModificacion = _usuarioService.UsuarioNombres;

                    _appDbContext.GestionTurnos.Update(gestionTurnoExistente);
                    await _appDbContext.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Turno actualizado exitosamente.";
                    return RedirectToAction(nameof(listaAsignacionTurno));
                }

                cargarListasDeSeleccion();
                return View(gestionTurno);
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Ocurrió un error al actualizar el turno: " + ex.Message);
                cargarListasDeSeleccion();
                return View(gestionTurno);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EliminarTurnoConfirmado(int id)
        {
            try
            {
                var gestionTurno = await _appDbContext.GestionTurnos.FindAsync(id);

                if (gestionTurno == null)
                {
                    return NotFound();
                }

                _appDbContext.GestionTurnos.Remove(gestionTurno);
                await _appDbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Turno eliminado exitosamente.";
                return RedirectToAction(nameof(listaAsignacionTurno));
            }
            catch (DbUpdateException ex)
            {
                TempData["ErrorMessage"] = "Ocurrió un error al eliminar el turno: " + ex.Message;
                return RedirectToAction(nameof(listaAsignacionTurno));
            }
        }

        private void cargarListasDeSeleccion()
        {
            ViewBag.Turnos = new SelectList(_appDbContext.Turnos, "IdTurno", "NombreTurno");
            ViewBag.Personas = new SelectList(_appDbContext.Personas, "IdPersona", "NombreCompleto");
        }

        [HttpGet]
        public async Task<IActionResult> ListarTurnoPersona(int? pageNumber)
        {
            var nameIdentifier = _usuarioService.UsuarioId;

            var gestionTurnos = _appDbContext.GestionTurnos
                .Include(g => g.Persona)
                .Include(g => g.Turno)
                .Where(g => g.IdPersona == nameIdentifier);

            int pageSize = 10;
            var listaTurnos = await PaginatedList<GestionTurno>.CreateAsync(gestionTurnos.AsNoTracking(), pageNumber ?? 1, pageSize);
            return View(listaTurnos);
        }
    }
}
