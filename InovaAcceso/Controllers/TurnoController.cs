using Microsoft.AspNetCore.Mvc;
using InovaAcceso.Data;
using InovaAcceso.Models;
using Microsoft.EntityFrameworkCore;
using InovaAcceso.Filters;



namespace InovaAcceso.Controllers
{
    [AuthorizeSession("Admin")]
    public class TurnoController : Controller
    {
        private readonly AppDBContext _appDbContext;
        public TurnoController(AppDBContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> ListaTurnos(string searchString, int? pageNumber)
        {
            var turnos = from t in _appDbContext.Turnos
                          select t;
            if (!string.IsNullOrEmpty(searchString))
            {
                turnos = turnos.Where(s => s.NombreTurno.Contains(searchString));
            }
            int pageSize = 10;
            var paginatedList = await PaginatedList<Turno>.CreateAsync(turnos.AsNoTracking(), pageNumber ?? 1, pageSize);
            return View(paginatedList);
        }
        //Controla las funcionalidades de paginacion
        [HttpGet]
        public IActionResult NuevoTurno()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> NuevoTurno(Turno turnos)
        {
            await _appDbContext.Turnos.AddAsync(turnos);
            await _appDbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Turno creado exitosamente.";
            return RedirectToAction(nameof(ListaTurnos));
        }

        [HttpGet]
        public async Task<IActionResult> EditarTurno(int id)
        {
            Turno turnos = await _appDbContext.Turnos.FirstAsync(c => c.IdTurno == id);
            return View(turnos);
        }

        [HttpPost]
        public async Task<IActionResult> EditarTurno(Turno turnos)
        {
            if (ModelState.IsValid)
            {
                _appDbContext.Turnos.Update(turnos);
                await _appDbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Turno actualizado exitosamente.";
                return RedirectToAction(nameof(ListaTurnos));
            }
            TempData["ErrorMessage"] = "Ocurrió un error al actualizar el turno. Por favor, inténtalo de nuevo.";
            return View(turnos);
        }

        [HttpGet]
        public async Task<IActionResult> EliminarTurnos(int id)
        {
            try
            {
                Turno turnos = await _appDbContext.Turnos.FirstAsync(c => c.IdTurno == id);
                _appDbContext.Turnos.Remove(turnos);
                await _appDbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Turno eliminado exitosamente.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Ocurrió un error al eliminar el turno.";
            }
            return RedirectToAction(nameof(ListaTurnos));
        }

    }
}
