using System;

namespace InovaAcceso.Models
{
    public class TipoDocumento
    {
		
		public int IdTipoDoc {  get; set; }
		public required string Documento { get; set; }
		public DateTime FechaCreacion {  get; set; }
        public DateTime FechaModificacion { get; set; }
        public required string ResponsableModificacion { get; set; }
    }
}
