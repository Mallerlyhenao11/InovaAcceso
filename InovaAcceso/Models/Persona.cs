using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Principal;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace InovaAcceso.Models
{
    public class Persona
    {
        public int IdPersona { get; set; }
        public int IdTipoDoc { get; set; }
        // Propiedades de navegación
        public virtual required TipoDocumento TipoDocumento { get; set; }
        public int NumeroDocumento { get; set; }
        public required string PrimerNombre { get; set; }
        public  string SegundoNombre { get; set; }
        public required string PrimerApellido { get; set; }
        public  string SegundoApellido { get; set; }
        public DateOnly FechaNacimiento { get; set; }
        public int Edad { get; set; }
        public required string Sexo { get; set; }
        public DateOnly FechaIngreso { get; set; }
        public required string Direccion { get; set; }
        public string Telefono { get; set; }
        public required string Email { get; set; }

        public required string Contrasena { get; set; }
        public bool Restablecer{ get; set; }
        public int IdCargo { get; set; }
        // Propiedades de navegación
        public virtual required Cargo Cargo { get; set; }
        public int IdEstado { get; set; }
        // Propiedades de navegación
        public virtual required Estado Estado { get; set; }
        public int IdRol { get; set; }
        // Propiedades de navegación
        public virtual required Rol Rol { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public required string ResponsableModificacion { get; set; }
        public string NombreCompleto => $"{PrimerNombre} {SegundoNombre} {PrimerApellido} {SegundoApellido}".Trim();

    }
}
