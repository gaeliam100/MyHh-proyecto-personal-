using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication2.Models;

namespace WebApplication2.Models
{
    public class Comentario
    {
        [Key]
        public int ComentarioID { get; set; }
        [Required]
        public string comentario { get; set; }
        [Required]
        public int IdUsuario { get; set; }
        [Required]
        public int Publicacionid { get; set; }

        public DateTime Fecha { get; set; }
        [ForeignKey("Id Usuario")]
        public User usua { get; set; }
        [ForeignKey("Publi ID")]
        public Publicacion Publi { get; set; }
    }
}
