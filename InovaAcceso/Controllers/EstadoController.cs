using InovaAcceso.Data;
using InovaAcceso.Filters;
using InovaAcceso.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InovaAcceso.Controllers
{
    [AuthorizeSession("Admin")]
    public class EstadoController : Controller
    {
        private readonly AppDBContext _appDbContext;
        public EstadoController(AppDBContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        [HttpGet]
        public async Task<IActionResult> ListaEstado(string searchString, int? pageNumber)
        {
            var estados = from e in _appDbContext.Estados
                         select e;
            if (!string.IsNullOrEmpty(searchString))
            {
                estados = estados.Where(s => s.NombreEstado.Contains(searchString));
            }
            int pageSize = 10;
            var paginatedList = await PaginatedList<Estado>.CreateAsync(estados.AsNoTracking(), pageNumber ?? 1, pageSize);
            return View(paginatedList);
        }

        //Controla las funcionalidades de paginacion
        [HttpGet]
        public async Task<IActionResult> ListaCargo(string searchString, int? pageNumber)
        {
            var cargos = from c in _appDbContext.Cargos
                         select c;

            if (!string.IsNullOrEmpty(searchString))
            {
                cargos = cargos.Where(s => s.NombreCargo.Contains(searchString));
            }

            int pageSize = 10;
            var paginatedList = await PaginatedList<Cargo>.CreateAsync(cargos.AsNoTracking(), pageNumber ?? 1, pageSize);
            return View(paginatedList);
        }
        // Controla la funcionalidad de Listar cargos

        [HttpGet]
        public IActionResult NuevoEstado()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NuevoEstado(Estado estado)
        {
            if (ModelState.IsValid)
            {
                await _appDbContext.Estados.AddAsync(estado);
                await _appDbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Estado agregado exitosamente.";
                return RedirectToAction(nameof(ListaEstado));
            }
            TempData["ErrorMessage"] = "Ocurrió un error al guardar el estado. Por favor, inténtalo de nuevo.";
            return View(estado);
        }

        [HttpGet]
        public async Task<IActionResult> EditarEstado(int id)
        {
            Estado estado = await _appDbContext.Estados.FirstAsync(c => c.IdEstado == id);
            return View(estado);
        }

        [HttpPost]
        public async Task<IActionResult> EditarEstado(Estado estado)
        {
            if (ModelState.IsValid)
            {
                _appDbContext.Estados.Update(estado);
                await _appDbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Estado actualizado exitosamente.";

                return RedirectToAction(nameof(ListaEstado));
            }
            TempData["ErrorMessage"] = "Ocurrió un error al actualizar el cargo. Por favor, inténtalo de nuevo.";
            return View(ListaEstado);
        }
        [HttpGet]
        public async Task<IActionResult> EliminarEstado(int id)
        {
            try
            { 
            Estado estado = await _appDbContext.Estados.FirstAsync(c => c.IdEstado == id);
            _appDbContext.Estados.Remove(estado);
            await _appDbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Estado eliminado exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ocurrió un error al intentar eliminar el estado: " + ex.Message;
            }

            return RedirectToAction(nameof(ListaEstado));
        }

    }
}
