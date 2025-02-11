using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Principal;
using System;

namespace InovaAcceso.Models
{
    public class GestionTurno
	{
        
        public int IdGestionTurno { get; set; }
		public int IdPersona { get; set; }
        // Propiedades de navegación
        public virtual required Persona Persona { get; set; }
        public int IdTurno { get; set; }
        // Propiedades de navegación
        public virtual required Turno Turno { get; set; }
        public DateTime FechaInicio { get; set; }
		public DateTime FechaFin { get; set; }
		public DateTime FechaCreacion { get; set; }
		public DateTime FechaModificacion { get; set; }
		public required string ResponsableModificacion { get; set; }
	}
}
