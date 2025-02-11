using Microsoft.AspNetCore.Mvc;
using InovaAcceso.Data;
using InovaAcceso.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InovaAcceso.Controllers
{
    //[Authorize]
    public class GestionTurnoController : Controller
	{
		private readonly AppDBContext _appDbContext;

		public GestionTurnoController(AppDBContext appDbContext)
		{
			_appDbContext = appDbContext;
		}

		[HttpGet]
		public async Task<IActionResult> listaAsignacionTurno(string searchString, int? pageNumber)
		{
			var gestionTurnos = _appDbContext.GestionTurnos
								.Include(g => g.Persona)
								.Include(g => g.Turno)
								.AsQueryable();

			// Aplicar filtro de búsqueda si se proporciona un término de búsqueda
			if (!string.IsNullOrEmpty(searchString))
			{
				gestionTurnos = gestionTurnos.Where(g => g.Persona.PrimerNombre.ToString().Contains(searchString) ||
														 g.Persona.PrimerApellido.ToString().Contains(searchString) ||
														 g.Turno.NombreTurno.ToString().Contains(searchString));
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
            
                await _appDbContext.GestionTurnos.AddAsync(gestionTurno);
                await _appDbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Turno asigando exitosamente.";
                return RedirectToAction(nameof(listaAsignacionTurno));
            
        }

            [HttpGet]
        public async Task<IActionResult> editarAsignacionTurno(int id)
        {
			cargarListasDeSeleccion();
            GestionTurno gestionTurno = await _appDbContext.GestionTurnos.FirstAsync(g => g.IdGestionTurno == id);
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

            var gestionTurnoExistente = await _appDbContext.GestionTurnos.FirstOrDefaultAsync(g => g.IdGestionTurno == gestionTurno.IdGestionTurno);
            if (gestionTurnoExistente != null)
            {
                gestionTurnoExistente.IdPersona = gestionTurno.IdPersona;
                gestionTurnoExistente.IdTurno = gestionTurno.IdTurno;
                gestionTurnoExistente.FechaInicio = gestionTurno.FechaInicio;
                gestionTurnoExistente.FechaFin = gestionTurno.FechaFin;

                _appDbContext.GestionTurnos.Update(gestionTurnoExistente);
                await _appDbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Turno actualizado exitosamente.";
                return RedirectToAction(nameof(listaAsignacionTurno));
            }

            cargarListasDeSeleccion();
            return View(gestionTurno);
        }

        [HttpGet]
        public async Task<IActionResult> EliminarTurnoConfirmado(int id)
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




        private void cargarListasDeSeleccion()
        {
            ViewBag.Turnos = new SelectList(_appDbContext.Turnos, "IdTurno", "NombreTurno");
            ViewBag.Personas = new SelectList(_appDbContext.Personas, "IdPersona", "NombreCompleto");
        }

    }
}
