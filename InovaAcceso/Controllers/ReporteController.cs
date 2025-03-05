using InovaAcceso.Models;
using InovaAcceso.Data;
using Microsoft.AspNetCore.Mvc;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore;
using InovaAcceso.Filters;
using InovaAcceso.Service;
using ClosedXML.Excel;

namespace InovaAcceso.Controllers
{
    /// <summary>
    /// ReporteController maneja la generación de reportes para empleados y organizaciones.
    /// </summary>
    public class ReporteController : Controller
    {
        private readonly IConverter _converter;
        private readonly ILogger<ReporteController> _logger;
        private readonly AppDBContext _appDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UsuarioService _usuarioService;

        public ReporteController(IConverter converter,ILogger<ReporteController> logger, AppDBContext context, IHttpContextAccessor httpContextAccessor, UsuarioService usuarioService)
        {
            _httpContextAccessor = httpContextAccessor;
            _converter = converter;
            _logger = logger;
            _appDbContext = context;
            _usuarioService = usuarioService;
        }

        [AuthorizeSession("Admin")]
        public IActionResult Reportes()
        {
            return View();
        }

        // Reportes de Turno
        public IActionResult ReporteDeTurnos()
        {
            string urlReporte = GenerarUrlReporte("Reporte/VistaPDFReporteTurnos");
            var usuarioEmail = ObtenerUsuarioEmail();

            if (string.IsNullOrEmpty(usuarioEmail))
            {
                _logger.LogError("El usuario no tiene un email en sesión.");
                return BadRequest("No se encontró un usuario en sesión.");
            }

            var pdf = new HtmlToPdfDocument
            {
                GlobalSettings = new GlobalSettings
                {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait
                },
                Objects =
                {
                    new ObjectSettings { Page = urlReporte }
                }
            };

            var archivoPDF = _converter.Convert(pdf);
            return File(archivoPDF, "application/pdf");
        }
        public async Task<IActionResult> VistaPDFReporteTurnos()
        {
            var usuarioEmail = ObtenerUsuarioEmail();

            if (string.IsNullOrEmpty(usuarioEmail))
            {
                _logger.LogError("El usuario no tiene un email en sesión.");
                return BadRequest("No se encontró un usuario en sesión.");
            }

            _logger.LogInformation($"Generando reporte para el usuario: {usuarioEmail}");

            var gestionTurnos = await _appDbContext.GestionTurnos
                .Include(g => g.Persona)
                .Include(g => g.Turno)
                .Where(g => g.Persona.Email == usuarioEmail)
                .AsNoTracking()
                .ToListAsync();

            return View(gestionTurnos);
        }

        // Certificado Laboral
        public IActionResult CertificadoLaboral()
        {
            string urlReporte = GenerarUrlReporte("Reporte/VistaPDFCertificadoLaboral");

            // Asegúrate de que la URL sea accesible
            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = new GlobalSettings()
                {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait
                },
                Objects = { new ObjectSettings() { Page = urlReporte } }
            };

            var archivoPDF = _converter.Convert(pdf); 
            return File(archivoPDF, "application/pdf");
        }
        public async Task<IActionResult> VistaPDFCertificadoLaboral()
        {
            try
            {
                string usuarioEmail = ObtenerUsuarioEmail();
                _logger.LogWarning($"Intentando generar certificado para el email: {usuarioEmail}");

                if (string.IsNullOrEmpty(usuarioEmail))
                {
                    _logger.LogError("Email de usuario no encontrado en los claims");
                    ViewBag.ErrorMessage = "No se pudo identificar al usuario actual";
                    return View(null);
                }

                var persona = await _appDbContext.Personas
                    .Include(r => r.TipoDocumento)
                    .Include(r => r.Cargo)
                    .Include(r => r.Rol)
                    .SingleOrDefaultAsync(r => r.Email == usuarioEmail);

                if (persona == null)
                {
                    _logger.LogError($"No se encontró persona con el email: {usuarioEmail}");
                    ViewBag.ErrorMessage = "No se encontró información del usuario en el sistema";
                    return View(null);
                }

                return View(persona);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al generar certificado: {ex.Message}");
                ViewBag.ErrorMessage = "Ocurrió un error al generar el certificado";
                return View(null);
            }
        }

        // Reporte Cumplimiento Laboral
        public async Task<IActionResult> ReporteCumplimientoLaboral(string numeroDoc, DateTime fecha)
        {
            var registros = _appDbContext.RegistroAsistencias
                .Include(r => r.Persona)
                .Include(r => r.Turno)
                .AsQueryable();

            if (!string.IsNullOrEmpty(numeroDoc))
            {
                if (long.TryParse(numeroDoc, out long numDoc))
                {
                    registros = registros.Where(s => s.Persona.NumeroDocumento == numDoc);
                }
            }
            if (fecha != default(DateTime))
            {
                // Convertimos fecha a DateOnly para comparar correctamente con FechaIngreso
                DateOnly fechaFiltro = DateOnly.FromDateTime(fecha);
                registros = registros.Where(s => s.FechaIngreso == fechaFiltro);
            }



            var listaRegistros = await registros.ToListAsync(); // Se ejecuta la consulta

            return View(listaRegistros);
        }
        public IActionResult ExportarExcel(List<RegistroAsistencia> registros)
        {
            if (registros == null || !registros.Any())
            {
                return BadRequest("No hay datos para exportar.");
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Asistencias");
                int fila = 1;

                // Crear encabezados                
                worksheet.Cell(fila, 1).Value = "Nombre";
                worksheet.Cell(fila, 2).Value = "Turno";
                worksheet.Cell(fila, 3).Value = "Fecha Ingreso";
                worksheet.Cell(fila, 4).Value = "Hora Ingreso";
                worksheet.Cell(fila, 5).Value = "Hora Salida";
                worksheet.Cell(fila, 6).Value = "Llegada Tarde";
                worksheet.Cell(fila, 7).Value = "Responsable Modificación";

                fila++;

                // Llenar datos en las filas
                foreach (var registro in registros)
                {
                    worksheet.Cell(fila, 1).Value = registro.Persona?.NombreCompleto ?? "N/A";
                    worksheet.Cell(fila, 2).Value = registro.Turno?.NombreTurno ?? "N/A";
                    worksheet.Cell(fila, 3).Value = registro.FechaIngreso.ToString("yyyy-MM-dd");
                    worksheet.Cell(fila, 4).Value = registro.HoraIngreso.ToString();
                    worksheet.Cell(fila, 5).Value = registro.HoraSalida.ToString();
                    worksheet.Cell(fila, 6).Value = registro.LlegadaTarde ? "Sí" : "No";
                    worksheet.Cell(fila, 7).Value = registro.ResponsableModificacion ?? "N/A";

                    fila++;
                }

                // Ajustar columnas al contenido
                worksheet.Columns().AdjustToContents();

                // Guardar el archivo en memoria
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var contenido = stream.ToArray();
                    return File(contenido, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ReporteAsistencia.xlsx");
                }
            }
        }

        // Reporte de productividad
        public IActionResult ReporteProductividad()
        {
            string url_pagina = GenerarUrlReporte("Reporte/VistaPDFReporteProductividad");
            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = new GlobalSettings()
                {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait
                },
                Objects = {
                 new ObjectSettings(){
                    Page = url_pagina
                 }
                }
            };
            var archivoPDF = _converter.Convert(pdf);
            return File(archivoPDF, "application/pdf");
        }
        public IActionResult VistaPDFReporteProductividad()
        {
            DateTime fechaFiltro = DateTime.Now;

            // Obtener empleados programados hoy, agrupados por turno
            var empleadosProgramadosPorTurno = _appDbContext.GestionTurnos
                .Include(g => g.Persona)
                .Include(g => g.Turno)
                .Where(g => fechaFiltro >= g.FechaInicio && fechaFiltro <= g.FechaFin) // Turnos activos hoy
                .ToList()
                .GroupBy(g => g.Turno.IdTurno)  // Agrupar por IdTurno
                .ToDictionary(g => g.Key, g => g.ToList()); // Diccionario por turno

            // Obtener registros de asistencia de hoy
            var registrosAsistencia = _appDbContext.RegistroAsistencias
                .Include(r => r.Persona)
                .Include(r => r.Turno)
                .Where(r => r.FechaIngreso == DateOnly.FromDateTime(fechaFiltro))
                .ToList();

            // Lista para almacenar los datos por turno
            var productividadPorTurno = new List<ProductividadModel>();
            int totalEmpleadosGlobal = 0;
            double sumaProductividadPonderada = 0;
            // Iterar sobre cada turno programado
            foreach (var turno in empleadosProgramadosPorTurno)
            {
                int idTurno = turno.Key;
                var empleadosProgramados = turno.Value; // Lista de empleados en este turno

                // Obtener solo los registros de asistencia de los empleados en este turno
                var empleadosProductivos = empleadosProgramados
                    .Where(ep => registrosAsistencia.Any(ra =>
                        ra.Persona.IdPersona == ep.Persona.IdPersona &&
                        ra.IdTurno == idTurno &&  // Asegurar que es del mismo turno
                        ra.HoraIngreso <= ep.Turno.HoraIngreso &&
                        ra.HoraSalida <= ep.Turno.HoraSalida))
                    .ToList();

                int totalProgramados = empleadosProgramados.Count;
                int totalProductivos = empleadosProductivos.Count;

                double porcentajeProductividad = totalProgramados > 0
                    ? Math.Round((totalProductivos / (double)totalProgramados) * 100, 2)
                    : 0;

                productividadPorTurno.Add(new ProductividadModel
                {
                    NombreTurno = empleadosProgramados.First().Turno.NombreTurno,
                    TotalEmpleados = totalProgramados,
                    TotalProductivos = totalProductivos,
                    PorcentajeProductividad = porcentajeProductividad
                });
                // Acumulamos los valores para el cálculo global
                totalEmpleadosGlobal += totalProgramados;
                sumaProductividadPonderada += porcentajeProductividad * totalProgramados;
            }
            // Cálculo del promedio ponderado
            double productividadTotal = totalEmpleadosGlobal > 0
                ? sumaProductividadPonderada / totalEmpleadosGlobal
                : 0;

            ViewBag.ProductividadTotal = productividadTotal; // Enviar a la vista

            return View(productividadPorTurno);
        }
        //Grafico asistencia
        [HttpGet]


        // GRAFICOS
        public JsonResult ObtenerDatosGrafico(DateTime fechaInicio, DateTime fechaFin)
        {
            var datos = _appDbContext.RegistroAsistencias
                .Where(r => r.FechaIngreso >= DateOnly.FromDateTime(fechaInicio) && r.FechaIngreso <= DateOnly.FromDateTime(fechaFin))
                .GroupBy(r => r.FechaIngreso)
                .AsEnumerable() // 🔹 Forzar la evaluación en memoria
                .Select(g => new
                {
                    Dia = g.Key.ToString("yyyy-MM-dd"), // 🔹 Ahora se puede usar ToString()
                    LlegadasTemprano = g.Count(r => !r.LlegadaTarde),
                    LlegadasTarde = g.Count(r => r.LlegadaTarde)
                })
                .OrderBy(d => d.Dia) // Ordenar por fecha
                .ToList();

            return Json(datos);
        }
        public IActionResult GraficoAsistencia()
        {
            return View();
        }

        //Comunees
        private string ObtenerUsuarioEmail()
        {
            return _usuarioService.UsuarioEmail;
        }
        /// Genera la URL base para acceder al reporte
        private string GenerarUrlReporte(string endpoint)
        {
            string paginaActual = HttpContext.Request.Path;
            string urlBase = HttpContext.Request.GetEncodedUrl().Replace(paginaActual, "");
            return $"{urlBase}/{endpoint}";
        }

        [AuthorizeSession("User")]
        public IActionResult ReportesUser()
        {
            return View();
        }

    }
}
