using Microsoft.AspNetCore.Mvc;
using InovaAcceso.Data;
using InovaAcceso.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using InovaAcceso.Filters;

namespace InovaAcceso.Controllers
{
    [AuthorizeSession("Admin")]
    public class AsistenciaController : Controller
    {
        private readonly AppDBContext _context;

        public AsistenciaController(AppDBContext context)
        {
            _context = context;
        }

        // Acción para mostrar la lista de registros de asistencia
        public async Task<IActionResult> ListaAsistencia(string searchString, int pageNumber = 1)
        {
            var registros = _context.RegistroAsistencias
                .Include(r => r.Persona) // Incluye la relación con Persona
                .Include(r => r.Turno)   // Incluye la relación con Turno
                .AsQueryable();

            // Filtrar por búsqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                registros = registros.Where(r =>
                    r.Persona.PrimerNombre.Contains(searchString) ||
                    r.Turno.NombreTurno.Contains(searchString));
            }

            // Paginación
            int pageSize = 10; // Número de registros por página
            var paginatedList = await PaginatedList<RegistroAsistencia>.CreateAsync(registros, pageNumber, pageSize);

            ViewData["CurrentFilter"] = searchString;
            return View(paginatedList);
        }

        // Acción para crear un nuevo registro de asistencia (GET)
        public IActionResult AgregarAsistencia()
        {
            CargarListasDesplegables();
            return View();
        }

        // Acción para crear un nuevo registro de asistencia (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarAsistencia(RegistroAsistencia registro)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(registro);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Asistencia agregada exitosamente.";
                    return RedirectToAction(nameof(ListaAsistencia));
                }
                catch (Exception ex)
                {
                    var innerException = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    TempData["ErrorMessage"] = $"Ocurrió un error al agregar la asistencia: {innerException}";
                }

            }

            CargarListasDesplegables(registro.IdPersona, registro.IdTurno);
            return View(registro);
        }

        // Acción para editar un registro de asistencia (GET)
        public async Task<IActionResult> EditarAsistencia(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registro = await _context.RegistroAsistencias.FindAsync(id);
            if (registro == null)
            {
                return NotFound();
            }

            CargarListasDesplegables(registro.IdPersona, registro.IdTurno);
            return View(registro);
        }

        // Acción para editar un registro de asistencia (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarAsistencia(int id, RegistroAsistencia registro)
        {
            if (id != registro.IdRegistro)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(registro);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Asistencia editada exitosamente.";
                    return RedirectToAction(nameof(ListaAsistencia));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RegistroAsistenciaExists(registro.IdRegistro))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Ocurrió un error al editar la asistencia: {ex.Message}";
                }
            }

            CargarListasDesplegables(registro.IdPersona, registro.IdTurno);
            return View(registro);
        }

        // Acción para eliminar un registro de asistencia (GET)
        public async Task<IActionResult> EliminarAsistencia(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var registro = await _context.RegistroAsistencias
                .Include(r => r.Persona)
                .Include(r => r.Turno)
                .FirstOrDefaultAsync(m => m.IdRegistro == id);

            if (registro == null)
            {
                return NotFound();
            }

            return View(registro);
        }

        // Acción para confirmar la eliminación de un registro de asistencia (POST)
        [HttpPost, ActionName("EliminarAsistencia")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarAsistenciaConfirmed(int id)
        {
            try
            {
                var registro = await _context.RegistroAsistencias.FindAsync(id);
                _context.RegistroAsistencias.Remove(registro);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Asistencia eliminada exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ocurrió un error al eliminar la asistencia: {ex.Message}";
            }

            return RedirectToAction(nameof(ListaAsistencia));
        }

        // Método privado para cargar listas desplegables (Personas y Turnos)
        private void CargarListasDesplegables(int? personaSeleccionada = null, int? turnoSeleccionado = null)
        {
            ViewBag.Personas = new SelectList(_context.Personas, "IdPersona", "PrimerNombre", personaSeleccionada);
            ViewBag.Turnos = new SelectList(_context.Turnos, "IdTurno", "NombreTurno", turnoSeleccionado);
        }

        // Método privado para verificar si un registro de asistencia existe
        private bool RegistroAsistenciaExists(int id)
        {
            return _context.RegistroAsistencias.Any(e => e.IdRegistro == id);
        }
    }
}
