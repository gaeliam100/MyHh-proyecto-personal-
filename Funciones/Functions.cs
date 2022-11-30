using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using System.Net.Mail;

namespace WebApplication2.Funciones
{
    public class Functions
    {
        private SmtpClient cliente;
        private MailMessage email;

        private string _Host = "smtp.gmail.com";
        private int _Port = 587;
        private string _User = "webpagemystery@gmail.com";
        private string _contraseña = "nbqqcoxvealypjne";
        private bool _Ssl = true;
        public Functions()
        {
            cliente = new SmtpClient(_Host, _Port)
            {
                EnableSsl = _Ssl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_User, _contraseña)
            };
        }
        public static string EncriptarContraseña(string Contra)
        {
            using (SHA256 encriptar = SHA256.Create())
            {
                byte[] bytes = encriptar.ComputeHash(Encoding.UTF8.GetBytes(Contra));

                StringBuilder builder = new StringBuilder();

                for (int i=0;i<bytes.Length;i++)
                {
                    builder.Append(bytes[i].ToString("Secret"));
                }
                return builder.ToString();
            }
        }

        public static int GenerarCode(int Longitudcontra=10)
        {
            Random CodigoA = new Random();
            string Cadena = "0123456789";
            int Longitud = Cadena.Length;
            char L;
            string GenCode=string.Empty;
            for (int i=0;i<Longitudcontra;i++)
            {
                L = Cadena[CodigoA.Next(Longitud)];
                GenCode += L.ToString();
            }
            
            return int.Parse( GenCode);
        }

        public void SendCorreo(string destinatario,string asunto,string mensaje,bool esHtml=true)
        {
            email = new MailMessage(_User, destinatario, asunto, mensaje);
            email.IsBodyHtml = esHtml;
            cliente.Send(email);
        }
        public void SendCorreo(MailMessage message)
        {
            cliente.Send(message);
        }
        public static void SendE(string destinatario, string asunto, string mensaje, bool esHtml = true)
        {
            try
            {
                var correo = new Functions();
                correo.SendCorreo(destinatario, asunto, mensaje, esHtml);
            }catch(Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        //CODIGO QUE NO ES DE MI PROPIEDAD
        public static string EncriptarPassword(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static string PasswordSeguro(int longitudContrasenia = 6)
        {
            Random rdn = new Random();
            string caracteres = "1234567890";
            int longitud = caracteres.Length;
            char letra;
            string contraseniaAleatoria = string.Empty;
            for (int i = 0; i < longitudContrasenia; i++)
            {
                letra = caracteres[rdn.Next(longitud)];
                contraseniaAleatoria += letra.ToString();
            }
            return contraseniaAleatoria;
        }
    }
}
