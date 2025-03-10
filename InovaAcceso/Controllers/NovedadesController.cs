using Microsoft.AspNetCore.Mvc;
using InovaAcceso.Data;
using InovaAcceso.Models;
using Microsoft.EntityFrameworkCore;
using InovaAcceso.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InovaAcceso.Controllers
{
    [AuthorizeSession("Admin")]
    public class NovedadController : Controller
    {
        private readonly AppDBContext _appDbContext;

        public NovedadController(AppDBContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        // Controla las funcionalidades de paginación
        [HttpGet]
        public async Task<IActionResult> ListaNovedad(string searchString, int? pageNumber)
        {
            try
            {
                var novedades = _appDbContext.Novedades
                    .Include(n => n.Persona) // Incluye la relación con Persona
                    .Include(n => n.Estado) // Incluye la relación con Estado
                    .AsQueryable();

                // Filtra por búsqueda si se proporciona
                if (!string.IsNullOrEmpty(searchString))
                {
                    novedades = novedades.Where(n => n.Descripcion.Contains(searchString));
                }

                // Configuración de paginación
                int pageSize = 10;
                pageNumber = pageNumber <= 0 ? 1 : pageNumber; // Asegura que el número de página sea válido
                var paginatedList = await PaginatedList<Novedad>.CreateAsync(novedades.AsNoTracking(), pageNumber ?? 1, pageSize);

                return View(paginatedList);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ocurrió un error al cargar la lista de novedades: " + ex.Message;
                return View(new List<Novedad>()); // Devuelve una lista vacía en caso de error
            }
        }

        // Controla la funcionalidad de crear una nueva novedad (GET)
        [HttpGet]
        public IActionResult NuevaNovedad()
        {
            PoblarViewData();
            return View();
        }

        // Controla la funcionalidad de crear una nueva novedad (POST)
        [HttpPost]
        public async Task<IActionResult> NuevaNovedad(Novedad novedad)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    novedad.FechaCreacion = DateTime.Now;
                    await _appDbContext.Novedades.AddAsync(novedad);
                    await _appDbContext.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Novedad agregada exitosamente.";
                    return RedirectToAction(nameof(ListaNovedad));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Ocurrió un error al guardar la novedad: " + ex.Message;
                }
            }

            PoblarViewData(novedad);
            return View(novedad);
        }

        // Controla la funcionalidad de editar una novedad (GET)
        [HttpGet]
        public async Task<IActionResult> EditarNovedad(int id)
        {
            var novedad = await _appDbContext.Novedades
                .Include(n => n.Persona)
                .Include(n => n.Estado)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (novedad == null)
            {
                TempData["ErrorMessage"] = "La novedad no fue encontrada.";
                return RedirectToAction(nameof(ListaNovedad));
            }

            PoblarViewData(novedad);
            return View(novedad);
        }

        // Controla la funcionalidad de editar una novedad (POST)
        [HttpPost]
        public async Task<IActionResult> EditarNovedad(Novedad novedad)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    novedad.FechaModificacion = DateTime.Now;
                    _appDbContext.Novedades.Update(novedad);
                    await _appDbContext.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Novedad actualizada exitosamente.";
                    return RedirectToAction(nameof(ListaNovedad));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NovedadExists(novedad.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Ocurrió un error de concurrencia al actualizar la novedad.";
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Ocurrió un error al actualizar la novedad: " + ex.Message;
                }
            }

            PoblarViewData(novedad);
            return View(novedad);
        }

        // Controla la funcionalidad de eliminar una novedad (POST)
        [HttpPost]
        public async Task<IActionResult> EliminarNovedad(int id)
        {
            try
            {
                var novedad = await _appDbContext.Novedades.FirstOrDefaultAsync(n => n.Id == id);
                if (novedad == null)
                {
                    TempData["ErrorMessage"] = "La novedad no fue encontrada.";
                    return RedirectToAction(nameof(ListaNovedad));
                }

                _appDbContext.Novedades.Remove(novedad);
                await _appDbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Novedad eliminada exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ocurrió un error al intentar eliminar la novedad: " + ex.Message;
            }

            return RedirectToAction(nameof(ListaNovedad));
        }

        // Controla la funcionalidad de ver los detalles de una novedad
        [HttpGet]
        public async Task<IActionResult> DetallesNovedad(int id)
        {
            var novedad = await _appDbContext.Novedades
                .Include(n => n.Persona) // Incluye los datos del empleado
                .Include(n => n.Estado) // Incluye el estado de la novedad
                .FirstOrDefaultAsync(n => n.Id == id);

            if (novedad == null)
            {
                TempData["ErrorMessage"] = "La novedad no fue encontrada.";
                return RedirectToAction(nameof(ListaNovedad));
            }

            return View(novedad);
        }

        // Método auxiliar para poblar ViewData
        private void PoblarViewData(Novedad novedad = null)
        {
            ViewData["IdEstado"] = new SelectList(_appDbContext.Estados, "IdEstado", "NombreEstado", novedad?.IdEstado);
            ViewData["IdPersona"] = new SelectList(_appDbContext.Personas, "IdPersona", "PrimerNombre", novedad?.IdPersona);
        }

        // Método auxiliar para verificar si existe una novedad
        private bool NovedadExists(int id)
        {
            return _appDbContext.Novedades.Any(e => e.Id == id);
        }
    }
}