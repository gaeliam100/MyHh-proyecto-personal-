using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class RegistroController : Controller
    {
        private readonly ContexDB _context;

        public RegistroController(ContexDB contexDB)
        {
            _context = contexDB;
        }
        // GET: RegistroController
        public ActionResult Principal()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Formulario()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Formulario([Bind("IdUsuario,Usuario,Edad,Contraseña,Correo,Imagen,FechaAlta,FechaUltimaActualizacion,codigo")] User Usuar)
        {

            Usuar.FechaAlta = DateTime.Now;
            Usuar.FechaUltimaActualizacion = DateTime.Now;
            Usuar.Imagen = "pendiente.jpg";
           Usuar.Contraseña = Funciones.Functions.EncriptarPassword(Usuar.Contraseña);
            Usuar.codigo = Funciones.Functions.PasswordSeguro();
            //if (ModelState.IsValid)
            //{
                _context.Add(Usuar);
                await _context.SaveChangesAsync();
            var mensaje = string.Format("<H1>Registro exitoso</H1><p>Se Completo Correctamente tu Registro su codigo de verificación es: <strong>" + Usuar.codigo.ToString() + "</strong></p>", Usuar.Usuario.ToString()); ;
                try
                {
                    Funciones.Functions.SendE(Usuar.Correo,"Registro completado",mensaje);
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Principal",new { completo=true,correo=false});
                }
            var Usuario = _context.Usuario.FirstOrDefault(u => u.Usuario==Usuar.Usuario);
            List<Claim> autorizacion = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,Usuario.Usuario),
                new Claim(ClaimTypes.Email,Usuario.Correo),
                new Claim(ClaimTypes.SerialNumber,Usuario.codigo),
                new Claim(ClaimTypes.Role,"Usuario"),
                new Claim("UsuarioID", Usuario.IdUsuario.ToString()),
                new Claim("Autenticado","SI"),
            };
            var identidad = new ClaimsIdentity(autorizacion, "AutorizacionesClaim");
            var principal = new ClaimsPrincipal(new[] { identidad });
            await HttpContext.SignInAsync(principal);
            return RedirectToActionPermanent("Confirmacion",new{user=Usuar.Usuario});

            //}
          //  return View(); 
        }
        [HttpGet]
        public IActionResult Confirmacion(string user)
        {
            var Usuario = _context.Usuario.FirstOrDefault(u => u.Usuario == user);
            ViewBag._NomUsu = Usuario.Usuario;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Confirmacion(string uSer,string code)
        {
            var UserN = _context.Usuario.FirstOrDefault(u => u.Usuario == uSer);
            if (UserN.codigo!=code)
            {
                return View("Confirmacion",new {error=true});
            }
            else
            {
                var Usuario = _context.Usuario.FirstOrDefault(u => u.Usuario == UserN.Usuario);
                List<Claim> autorizacion = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,UserN.Usuario),
                new Claim(ClaimTypes.Email,UserN.Correo),
                new Claim(ClaimTypes.Role,"Usuario"),
                new Claim("UsuarioID", UserN.IdUsuario.ToString()),
                new Claim("Autenticado","SI"),
            };
                var identidad = new ClaimsIdentity(autorizacion, "AutorizacionesClaim");
                var principal = new ClaimsPrincipal(new[] { identidad });
                await HttpContext.SignInAsync(principal);
                UserN.codigo = "Verificado";
                _context.Update(UserN);
                _context.SaveChanges();
                return RedirectPermanent("/Funciones/Login");
            }
        }
    }
}
