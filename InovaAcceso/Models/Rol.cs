using System;

namespace InovaAcceso.Models
{
    public class Rol
    {
    public int IdRol { get; set; }
    public required string NombreRol { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaModificacion { get; set; }
    public required string ResponsableModificacion { get; set; }

    }

}
