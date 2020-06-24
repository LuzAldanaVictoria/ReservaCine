using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Grupo3.ReservaDeCine.Controllers;
using Grupo3.ReservaDeCine.Database;
using Grupo3.ReservaDeCine.Extensions;
using Grupo3.ReservaDeCine.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Grupo3.ReservaDeCine.Models.Enums;

namespace ConSeguridad.Controllers
{
    public class CuentasController : Controller
    {
        private readonly CineDbContext _context;

        public CuentasController(CineDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Ingresar(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Ingresar(string username, string password, string returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))  // Se valida que se envie usuario y contraseña y que no lleguen vacios
            {
                Usuario usuario = _context.Clientes.FirstOrDefault(x => x.Username == username); // se le pide a la base de datos que encuentre al usuario con ese UserName
              
                if (usuario == null)  // aca se confirma que el usuario exista, sino.. da error
                {
                    usuario = _context.Administradores.FirstOrDefault(x => x.Username == username);
                }

                if (usuario != null)
                {
                    var passwordEncriptada = password.Encriptar();  // encripto la password del formulario con el metodo de extension

                    if (usuario.Password.SequenceEqual(passwordEncriptada)) //comparo cada uno de los elementos del array de la pasword del formulario, con la password de la base de datos
                    {
                        //aca se crea la identidad del usuario que esta ingresando:
                        // esto es para tener informacion relativa a la identidad del usuario, en el contexto actual(!)

                        ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                        identity.AddClaim(new Claim(ClaimTypes.Name, username));
                        identity.AddClaim(new Claim(ClaimTypes.Role, usuario.Role.ToString()));//De aca viene el [Authorize..role]
                        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()));
                        ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                        // en este paso se hace el login del usuario al sistema:
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                        usuario.FechaUltimoAcceso = DateTime.Now;
                        _context.SaveChanges();

                        if (!string.IsNullOrWhiteSpace(returnUrl))
                        {
                            return Redirect(returnUrl);

                        }

                        TempData["primerLogin"] = true;  // el tempData, vive dos request por lo que lo puedo usar entre metodos del controlador y levantarlo directamente

                        return RedirectToAction(nameof(HomeController.Index), "Home");
                    }
                }
            }
            ViewBag.Error = "Usuario y/o contraseña incorrectos";
            ViewBag.UserName = username;
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Salir()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        
        [HttpGet]
        public IActionResult NoAutorizado()
        {
            return View();
        }

    }
}