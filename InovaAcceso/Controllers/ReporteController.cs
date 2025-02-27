using InovaAcceso.Models;
using InovaAcceso.Data;
using Microsoft.AspNetCore.Mvc;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;



namespace InovaAcceso.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReporteController : Controller
    {
        private readonly IConverter _converter;
        private readonly ILogger<ReporteController> _logger;
        private readonly AppDBContext _context;


        public ReporteController(IConverter converter, ILogger<ReporteController> logger, AppDBContext context)

        {
            _converter = converter;
            _logger = logger;
            _context = context;
        }

        public IActionResult Reportes()
        {
            return View();
        }

        // --- Aquí agregamos la acción para mostrar los gráficos ---
        public IActionResult Graficos()
        {
            // Simulación de datos, pero ahora con la clase Reporte
            var asistencias = new List<Reporte>
    {
        new Reporte { Periodo = "1-15 Ene", Asistencias = 50 },
        new Reporte { Periodo = "16-31 Ene", Asistencias = 45 },
        new Reporte { Periodo = "1-15 Feb", Asistencias = 60 }
    };

            return View(asistencias);
        }
        // --- Aquí termina la acción para mostrar los gráficos ---

        public IActionResult ReporteProductividad()
        {
            // Simulación de datos de productividad
            var reportesProductividad = new List<Reporte>
    {
        new Reporte { Periodo = "1-15 Ene", Asistencias = 80 },
        new Reporte { Periodo = "16-31 Ene", Asistencias = 75 },
        new Reporte { Periodo = "1-15 Feb", Asistencias = 90 }
    };

            return View(reportesProductividad);
        }

        public IActionResult GenerarCertificadoLaboral()
        {
            return View("CertificadoLaboral");
        }


        public IActionResult VistaParaPDF()
        {
            return View();
        }

        public IActionResult MostrarPDFenPagina()
        {
            string pagina_actual = HttpContext.Request.Path;
            string url_pagina = HttpContext.Request.GetEncodedUrl();
            url_pagina = url_pagina.Replace(pagina_actual, "");
            url_pagina = $"{url_pagina}/Reporte/VistaParaPDF";

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

        public IActionResult DescargarPDF()
        {
            string pagina_actual = HttpContext.Request.Path;
            string url_pagina = HttpContext.Request.GetEncodedUrl();
            url_pagina = url_pagina.Replace(pagina_actual, "");
            url_pagina = $"{url_pagina}/Reporte/VistaParaPDF";

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
            string nombrePDF = "reporte_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".pdf";

            return File(archivoPDF, "application/pdf", nombrePDF);
        }
        // --- CERTIFICADO DE PRODUCTIVIDAD ---//
        // --- Aquí es para descargar la prudctividad en pdf ---

        public IActionResult VistaParaPDFProductividad()
        {
            return View();
        }
             

        public IActionResult GenerarCertificadoProductividad()
        {
            return View("VistaParaPDFProductividad");
        }

        public IActionResult DescargarPDFProductividad()
        {
            string pagina_actual = HttpContext.Request.Path;
            string url_pagina = HttpContext.Request.GetEncodedUrl();
            url_pagina = url_pagina.Replace(pagina_actual, "");
            url_pagina = $"{url_pagina}/Reporte/VistaParaPDFProductividad";

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = new GlobalSettings()
                {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait
                },
                Objects = {
            new ObjectSettings()
            {
                Page = url_pagina
            }
        }
            };

            var archivoPDF = _converter.Convert(pdf);
            string nombrePDF = "reporte_productividad_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".pdf";

            return File(archivoPDF, "application/pdf", nombrePDF);
        }


        public IActionResult MostrarPDFenPaginaProductividad()
        {
            string pagina_actual = HttpContext.Request.Path;
            string url_pagina = HttpContext.Request.GetEncodedUrl();
            url_pagina = url_pagina.Replace(pagina_actual, "");
            url_pagina = $"{url_pagina}/Reporte/VistaParaPDFProductividad";

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


        // --- CERTIFICADO REPORTE DE CUMPLIMIENTO LABORAL ---//
      
        public async Task<IActionResult> ReporteCumplimientoLaboral()
        {
         var registros = await _context.RegistroAsistencias
        .Include(r => r.Persona)
        .Include(r => r.Turno)
        .ToListAsync();

            return View(registros);
        }

       

        public IActionResult VistaParaPDFCumplimientoLaboral()
        {
            var registros = _context.RegistroAsistencias
                .Include(r => r.Persona)
                .Include(r => r.Turno)
                .ToList();

            return View(registros);
        }


        public IActionResult MostrarPDFenPaginaCumplimientoLaboral()
        {
            string pagina_actual = HttpContext.Request.Path;
            string url_pagina = HttpContext.Request.GetEncodedUrl();
            url_pagina = url_pagina.Replace(pagina_actual, "");
            url_pagina = $"{url_pagina}/Reporte/VistaParaPDFCumplimientoLaboral";

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

        public IActionResult ExportarExcel()
        {
            // Obtener los registros desde la base de datos
            var registros = _context.RegistroAsistencias
                .Include(r => r.Persona)
                .Include(r => r.Turno)
                .ToList();

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
                    worksheet.Cell(fila, 1).Value = registro.Persona.NombreCompleto; 
                    worksheet.Cell(fila, 2).Value = registro.Turno.NombreTurno;   
                    worksheet.Cell(fila, 3).Value = registro.FechaIngreso.ToString();
                    worksheet.Cell(fila, 4).Value = registro.HoraIngreso.ToString();
                    worksheet.Cell(fila, 5).Value = registro.HoraSalida.ToString();
                    worksheet.Cell(fila, 6).Value = registro.LlegadaTarde ? "Sí" : "No";
                    worksheet.Cell(fila, 7).Value = registro.ResponsableModificacion;

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


    public IActionResult VistaParaPDFReporteHorario()
        {
            var registros = _context.RegistroAsistencias
                .Include(r => r.Persona)
                .Include(r => r.Turno)
                .ToList();

            return View(registros);
        }


        public IActionResult MostrarPDFenReporteHorario()
        {
            string pagina_actual = HttpContext.Request.Path;
            string url_pagina = HttpContext.Request.GetEncodedUrl();
            url_pagina = url_pagina.Replace(pagina_actual, "");
            url_pagina = $"{url_pagina}/Reporte/VistaParaPDFReporteHorario";

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

    }
}