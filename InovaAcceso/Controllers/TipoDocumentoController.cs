using Microsoft.AspNetCore.Mvc;
using InovaAcceso.Data;
using InovaAcceso.Models;
using Microsoft.EntityFrameworkCore;
using InovaAcceso.Filters;

namespace InovaAcceso.Controllers
{
    [AuthorizeSession("Admin")]
    public class TipoDocumentoController : Controller
    {
		private readonly AppDBContext _appDbContext;
		public TipoDocumentoController(AppDBContext appDbContext)
		{
			_appDbContext = appDbContext;
		}


		[HttpGet]
		public async Task<IActionResult> ListaTipoDoc(string searchString, int? pageNumber)
		{
            var tipodoc = from t in _appDbContext.TipoDocumentos
                          select t;
            if (!string.IsNullOrEmpty(searchString))
            {
                tipodoc = tipodoc.Where(s => s.Documento.Contains(searchString));
            }
            int pageSize = 10;
            var paginatedList = await PaginatedList<TipoDocumento>.CreateAsync(tipodoc.AsNoTracking(), pageNumber ?? 1, pageSize);
            return View(paginatedList);
		}
        //Controla las funcionalidades de paginacion
        [HttpGet]
        public IActionResult NuevoTipoDoc()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> NuevoTipoDoc(TipoDocumento TipoDoc)
        {
            if (ModelState.IsValid)
            {
                await _appDbContext.TipoDocumentos.AddAsync(TipoDoc);
                await _appDbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tipo de documento agregado exitosamente.";

                return RedirectToAction(nameof(ListaTipoDoc));
            }
            TempData["ErrorMessage"] = "Ocurrió un error al guadar el tipo de documento. Por favor, inténtalo de nuevo.";
            return View(ListaTipoDoc);

        }
        [HttpGet]
        public async Task<IActionResult> EditarTipoDoc(int id)
        {
            TipoDocumento tipodoc = await _appDbContext.TipoDocumentos.FirstAsync(c => c.IdTipoDoc == id);
            return View(tipodoc);
        }

        [HttpPost]
        public async Task<IActionResult> EditarTipoDoc(TipoDocumento TipoDoc)
        {
            if (ModelState.IsValid)
            {
                _appDbContext.TipoDocumentos.Update(TipoDoc);
                await _appDbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tipo de documento actualizado exitosamente.";

                return RedirectToAction(nameof(ListaTipoDoc));
            }
            TempData["ErrorMessage"] = "Ocurrió un error al actualizar el tipo de documento. Por favor, inténtalo de nuevo.";
            return View(ListaTipoDoc);
        }
        [HttpGet]
        public async Task<IActionResult> EliminarTipoDoc(int id)
        {
            try
            {
                TipoDocumento tipodoc = await _appDbContext.TipoDocumentos.FirstAsync(c => c.IdTipoDoc == id);
                _appDbContext.TipoDocumentos.Remove(tipodoc);
                await _appDbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tipo de documento eliminado exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ocurrió un error al intentar eliminar el tipo de documento: " + ex.Message;
            }
            
            return RedirectToAction(nameof(ListaTipoDoc));
        }
    }
}
