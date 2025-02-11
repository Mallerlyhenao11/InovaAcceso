using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Principal;
using System;

namespace InovaAcceso.Models
{
    public class RegistroAsistencia
    {
        public required int IdRegistro { get; set; }
        public required int IdPersona { get; set; }
        // Propiedades de navegación
        public virtual required Persona Persona { get; set; }
        public required int IdTurno { get; set; }
        // Propiedades de navegación
        public virtual required Turno Turno { get; set; }
        public DateOnly FechaIngreso { get; set; }
        public TimeSpan HoraIngreso { get; set; }
        public TimeSpan HoraSalida { get; set; } /*HourAdjustmen*/
        public bool LlegadaTarde { get; set; } = false;
        public int Tardanza { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public required string ResponsableModificacion { get; set; }
    }
}