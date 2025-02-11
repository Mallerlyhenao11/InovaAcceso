using InovaAcceso.Models;
using Microsoft.AspNetCore.Mvc;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Http.Extensions;


namespace InovaAcceso.Controllers
{
    public class ReporteController : Controller
    {
        private readonly IConverter _converter;

        public ReporteController(IConverter converter)
        {
            _converter = converter;
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

        // --- Aquí es para descargar la prudctividad en pdf ---
        public IActionResult DescargarPDFProductividad()
        {
            string pagina_actual = HttpContext.Request.Path;
            string url_pagina = HttpContext.Request.GetEncodedUrl();
            url_pagina = url_pagina.Replace(pagina_actual, "");
            url_pagina = $"{url_pagina}/Reporte/ReporteProductividad";

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

    }
}
