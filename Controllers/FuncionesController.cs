using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class FuncionesController : Controller
    {
        private readonly ContexDB _context;
        private readonly ILogger<HomeController> _logger;
        private ClaimsPrincipal principal;
        private readonly IWebHostEnvironment _hostEnviroment;

        public FuncionesController(ContexDB contexDB, IWebHostEnvironment HostEnvironment,ILogger<FuncionesController> logger)
        {
            _context = contexDB;
            _hostEnviroment = HostEnvironment;
            //_logger = logger;
        }
        [HttpGet]
        public IActionResult Login()
        {
            ViewBag.Error = string.IsNullOrEmpty(HttpContext.Request.Query["error"]) ? false : true;
            ViewBag.Error = string.IsNullOrEmpty(HttpContext.Request.Query["error"]) ? false : true;
            ViewBag.Completo =string.IsNullOrEmpty(HttpContext.Request.Query["completo"]) ? false : true;
            return View();
        }
        [HttpPost]
        public async Task< IActionResult> Login(string Usuario,string contraseña)
        {
            var UserN = _context.Usuario.FirstOrDefault(u => u.Usuario == Usuario);
            if (UserN==null)
            {
                return RedirectToAction("Login",new {error=true });
            }
            if (UserN.Contraseña != Funciones.Functions.EncriptarPassword(contraseña))
            {
                return RedirectToAction("Login", new { error = true });
            }

                List<Claim> autorizacion = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,UserN.Usuario),
                new Claim(ClaimTypes.Email,UserN.Correo),
                new Claim(ClaimTypes.Role,"Usuario"),
                new Claim("IdUsuario", UserN.IdUsuario.ToString()),
                new Claim("Autenticado","SI"),
            };
                var identidad = new ClaimsIdentity(autorizacion, "AutorizacionesClaim");
                var principal = new ClaimsPrincipal(new[] { identidad });
                UserN.FechaUltimaActualizacion = DateTime.Now;
                _context.Update(UserN);
                await HttpContext.SignInAsync(principal);

                return RedirectToAction("Perfil");

        }
        [Authorize]
        public IActionResult perfilCLAIMS()
        {
            return View(User?.Claims);
        }
        public async Task<IActionResult> SingOut()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }
        public IActionResult Perfil()
        {
            var UsuarioID = Convert.ToInt32(User.Claims.FirstOrDefault(p => p.Type == "IdUsuario").Value);
            ViewBag._publicaciones = _context.Publicaciones.Where(p => p.IdUsuario == UsuarioID).ToList();
            ViewBag._Usuario = _context.Usuario.FirstOrDefault(u => u.IdUsuario == UsuarioID);
            return View();
        }
        [HttpGet]
        public IActionResult AgregarHistoria()
        {

            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult AgregarHistoria(IFormFile imagen,string contenido,string Titulo,string categoria)
        {
            var Historia = new Publicacion();
            var likes = new Like();
            string Camino = Path.Combine(_hostEnviroment.WebRootPath, "Recursos");
            string archivo = DateTime.Now.ToString("yymmd");
            try
            {
                var UsuarioID = Convert.ToInt32(User.Claims.FirstOrDefault(p => p.Type == "IdUsuario").Value);
                Historia.imagen = Path.Combine(Guid.NewGuid() + Path.GetExtension(imagen.FileName));
                Historia.IdUsuario = UsuarioID;
                Historia.content = contenido;
                Historia.visitas = 0;
                Historia.Titulo = Titulo;
                Historia.FechaPublicación = DateTime.Now;
                Historia.estado = "Pendiente";
                Historia.Categoria = categoria;
                likes.Estrella = 0;
                likes.IdUsuario = 0;
                if (!Directory.Exists(Path.Combine(Camino, archivo))) Directory.CreateDirectory(Path.Combine(Camino, archivo));

                using (var fstream = new FileStream(Path.Combine(Camino, Historia.imagen), FileMode.Create))
                {
                    imagen.CopyTo(fstream);
                }
                _context.Add(likes);
                _context.Add(Historia);
                _context.SaveChanges();
                return RedirectToAction("Perfil", new { error = false });
            }catch(Exception ex)
            {
                return RedirectToAction("AgregarHistoria", new { error = false });
            }
        }

        [HttpGet]
        public IActionResult Publicaciones(string InfoModal)
        {
            //var UsuarioID = Convert.ToInt32(User.Claims.FirstOrDefault(p => p.Type == "IdUsuario").Value);
            ViewBag._Like = _context.Likes.FirstOrDefault(p => p.IdUsuario==0);
            //var Like = _context.Likes.Where(p => p.Estrella == UsuarioID);
            ViewBag._Todo = _context.Publicaciones.Where(p=>p.estado=="Aprobado").ToList();
            //ViewBag._InfoModal = _context.Publicaciones.Where(p => p.idPublicación == int.Parse(InfoModal)).ToList();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Estrella(int id)
        {
            //try
            //{
           
                var Likes = new Like();
                var UsuarioID = Convert.ToInt32(User.Claims.FirstOrDefault(p => p.Type == "IdUsuario").Value);
                var u = _context.Publicaciones.FirstOrDefault(p => p.idPublicación == id);
                //var ne = _context.Likes.FirstOrDefault(p=>p.IdUsuario!=0);
                //else
                Likes.Estrella=1;
                Likes.IdUsuario = UsuarioID;
                Likes.Fecha = DateTime.Now;
                Likes.Publicacionid = id;
                u.like++;
                _context.Update(u);
                _context.Update(Likes);
                await _context.SaveChangesAsync();
                return RedirectToAction("Publicaciones", new { error = false });
            //}catch(Exception ex)
            //{
            //    return RedirectToAction("Login", new { error = true });
            //}
        }

        public IActionResult EditarPublicacion()
        {
            return View();
        }

        [HttpPost]
        public  IActionResult Comentario(string comentario)
        {

          return  RedirectToAction("_Comentario");
        }
        [HttpGet]        
        public IActionResult AprobarPublicación() 
        {
            ViewBag._Pendientes = _context.Publicaciones.Where(p => p.estado == "Pendiente");
            return View();
        }
        [HttpPost]
        public async Task< IActionResult> Aprobar(int id)
        {
            //var PubliID = Convert.ToInt32(_context.Publicaciones.FirstOrDefault(p => p.idPublicación == id));
           var ap= _context.Publicaciones.FirstOrDefault(p => p.idPublicación == id);
            if (ap.estado == "Pendiente")
            {
                ap.estado = "Aprobado";
                _context.Update(ap);
                await _context.SaveChangesAsync();
                return RedirectToActionPermanent("AprobarPublicación");
            }

            else
                return RedirectToAction("AprobarPublicación");
        }
    }
}
