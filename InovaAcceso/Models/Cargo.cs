using Microsoft.AspNetCore.Http.HttpResults;
using System;

namespace InovaAcceso.Models
{
    public class Cargo
    {

        public int IdCargo { get; set; }
        public required string NombreCargo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public required string ResponsableModificacion { get; set; }

    }
}