using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InovaAcceso.Models
{
    [Table("Huella")]
    public class Huella
    {
        [Key]
        public int IdHuella { get; set; }

        [ForeignKey("Persona")]
        [Required(ErrorMessage = "El ID de la persona es obligatorio.")]
        public int IdPersona { get; set; }

        [Required]
        public byte[] DatosHuella { get; set; }

        [Required]
        public DateTime FechaRegistro { get; set; }

        [Required]
        public bool Activo { get; set; }

        [Required]
        public DateTime FechaCreacion { get; set; }

        [Required]
        public DateTime FechaModificacion { get; set; }

        [Required(ErrorMessage = "El responsable de la modificación es obligatorio.")]
        public string ResponsableModificacion { get; set; }

        // Propiedad de navegación
        public virtual Persona Persona { get; set; }

        public Huella()
        {
            FechaRegistro = DateTime.Now;
            FechaCreacion = DateTime.Now;
            FechaModificacion = DateTime.Now;
            Activo = true;
        }
    }
}