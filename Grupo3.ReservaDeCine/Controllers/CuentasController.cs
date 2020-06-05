﻿using System;
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
        public IActionResult Ingresar(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Ingresar(string username, string password, string returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                Usuario usuario = _context.Usuarios.FirstOrDefault(x => x.Username == username);
                var passwordEncriptada = password.Encriptar();

                if (usuario.Password.SequenceEqual(passwordEncriptada))
                {
                    ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                    identity.AddClaim(new Claim(ClaimTypes.Name, username));
                    identity.AddClaim(new Claim(ClaimTypes.Role, usuario.Role.ToString()));
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()));
                    ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    usuario.FechaUltimoAcceso = DateTime.Now;
                    _context.SaveChanges();

                    if (!string.IsNullOrWhiteSpace(returnUrl))
                        return Redirect(returnUrl);

                    TempData["primerLogin"] = true;

                    return RedirectToAction(nameof(HomeController.Index), "Home");
                }
            }

            ViewBag.Error = "Usuario y/o contraseña incorrectos";
            ViewBag.UserName = username;
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.SelectRoles = new SelectList(Enum.GetNames(typeof(Role)), "Id");

            return View();
       
        }


        [HttpPost]
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