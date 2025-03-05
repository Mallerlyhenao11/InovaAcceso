using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace InovaAcceso.Models
{
    public class Novedad
    {
     
        public int Id { get; set; }
        public int IdEstado { get; set; }
        public virtual required Estado Estado { get; set; }
        public int IdPersona { get; set; }
        public virtual required Persona Persona { get; set; }
        public required DateTime FechaInicioNovedad { get; set; }
        public required DateTime FechaFinNovedad { get; set; }
        public string Descripcion { get; set; }
        public bool Aprobar { get; set; } = false;
        public required DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public required string ResponsableModificacion { get; set; }
    }
}
