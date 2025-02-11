using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Principal;
using System;

namespace InovaAcceso.Models
{
    public class Turno
    {
        public int IdTurno { get; set; }
        public required string NombreTurno { get; set; }
        public TimeSpan HoraIngreso { get; set; }
        public TimeSpan HoraSalida { get; set; }
        public int ToleranciaMinutos { get; set; }
        public int HorasTurno { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public required string ResponsableModificacion { get; set; }

    }

}