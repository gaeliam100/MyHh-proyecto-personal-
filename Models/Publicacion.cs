using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models
{
    public class Publicacion
    {
        [Key]
        public int idPublicación { get; set; }
        [Required]
        public int IdUsuario { get; set; }
        [Required]
        public string Titulo { get; set; }
        [Required]
        public string content { get; set; }
        [MaxLength(256)]
        public string imagen { get; set; }

        public string estado { get; set; }
        [Required]
        public string Categoria { get; set; }
        public int like { get; set; }
        [Required]
        public DateTime FechaPublicación { get; set; }
        public int visitas { get; set; }
        [ForeignKey("Id Usuario")]
        public User usuario { get; set; }

    }
}
