using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace WebApplication2.Models
{
    public class User
    {
        [Key]
        public int IdUsuario { get; set; }
        [Required(ErrorMessage ="El nombre de usuario es reqerido"), MaxLength(15), MinLength(5)]
        public string Usuario { get; set; }
       [Required,]
        public int Edad { get; set; }
        //[Required,MaxLength(8),MinLength(8),RegularExpression(@"^\$?\d+(\.(d{2}))?$")]
        public string Contraseña { get; set;}
        [Required,EmailAddress,RegularExpression(@"^\$?\d+(\.(d{2}))?$")]
        [MaxLength(100)]
        public string Correo { get; set; }
        public string Imagen { get; set; }
        [Required]
        public DateTime FechaAlta { get; set; }
        public DateTime FechaUltimaActualizacion { get; set; }
        public string codigo { get; set; }
    }
}
