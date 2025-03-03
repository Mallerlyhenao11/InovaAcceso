using Microsoft.AspNetCore.Mvc;
using InovaAcceso.Data;
using InovaAcceso.Models;
using Microsoft.EntityFrameworkCore;
using InovaAcceso.Filters;
namespace InovaAcceso.Controllers
{
    [AuthorizeSession("Admin")]
    public class CargoController : Controller
    {
        private readonly AppDBContext _appDbContext;
        public CargoController(AppDBContext appDbContext)
        {
            _appDbContext = appDbContext;
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
        public  IActionResult NuevoCargo()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NuevoCargo(Cargo cargo)
        {
            if (ModelState.IsValid)
            {
                await _appDbContext.Cargos.AddAsync(cargo);
                await _appDbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cargo agregado exitosamente.";
                return RedirectToAction(nameof(ListaCargo));
            }
            TempData["ErrorMessage"] = "Ocurrió un error al guadar el cargo. Por favor, inténtalo de nuevo.";
            return View(ListaCargo);

        }

        [HttpGet]
        public async Task<IActionResult> EditarCargo(int id)
        {
            Cargo cargo = await _appDbContext.Cargos.FirstAsync(c => c.IdCargo==id);
            return View(cargo);
        }

        [HttpPost]
        public async Task<IActionResult> EditarCargo(Cargo cargo)
        {
            if (ModelState.IsValid)
            {
                _appDbContext.Cargos.Update(cargo);
                await _appDbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cargo actualizado exitosamente.";

                return RedirectToAction(nameof(ListaCargo));
            }
            TempData["ErrorMessage"] = "Ocurrió un error al guadar el cargo. Por favor, inténtalo de nuevo.";
            return View(ListaCargo);
        }

        [HttpGet]
        public async Task<IActionResult> EliminarCargo(int id)
        {
            try
            {
                Cargo cargo = await _appDbContext.Cargos.FirstAsync(c => c.IdCargo == id);
                _appDbContext.Cargos.Remove(cargo);
                await _appDbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cargo eliminado exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ocurrió un error al intentar eliminar el cargo: " + ex.Message;
            }
            return RedirectToAction(nameof(ListaCargo));
        }

        }
    }

