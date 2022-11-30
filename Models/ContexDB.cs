using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebApplication2.Models
{
    public class ContexDB:DbContext
    {
        public ContexDB(DbContextOptions<ContexDB> opt) : base(opt) { }
        public DbSet<User> Usuario { get; set; }
        public DbSet<Publicacion> Publicaciones { get; set;}
        public DbSet<Like> Likes { get; set; }
    }
}
