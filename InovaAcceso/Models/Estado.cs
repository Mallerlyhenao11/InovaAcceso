using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Principal;
using System;

namespace InovaAcceso.Models
{
    public class Estado
    {
        public int IdEstado { get; set; }
        public required string NombreEstado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public required string ResponsableModificacion { get; set; }

    }
}
