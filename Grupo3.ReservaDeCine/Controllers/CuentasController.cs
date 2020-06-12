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
                Usuario usuario = _context.Clientes.FirstOrDefault(x => x.Username == username);

                if(usuario == null)
                {
                    usuario = _context.Administradores.FirstOrDefault(x => x.Username == username);
                    usuario.Role = Role.Administrador;
                }

                if (usuario != null)
                {
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
            }
            ViewBag.Error = "Usuario y/o contraseña incorrectos";
            ViewBag.UserName = username;
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }


        [HttpGet]
        public IActionResult Registrar()
        {
            return View();
       
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar (string password, Cliente cliente)
        {
            ComprobarFechaDeNacimiento(cliente);
            ValidarEmailExistente(cliente);
            ValidarUserNameExistente(cliente.Username);

            if (ModelState.IsValid)
            {
                cliente.Role = Role.Cliente;
                cliente.FechaDeAlta = DateTime.Now;
                cliente.Password = password.Encriptar();
                _context.Add(cliente);
                await _context.SaveChangesAsync();

                return RedirectToAction("Ingresar");
            }

            return View(cliente);
        }

   
        public IActionResult CreateExitoso(Cliente cliente)
        {
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


        private void ValidarUserNameExistente(string username)
        {
            if (_context.Usuarios.Any(x => Comparar(x.Username, username)))
            {
                ModelState.AddModelError(nameof(username), "Nombre de usuario no disponible");
            }
        }

        private void ComprobarFechaDeNacimiento(Cliente cliente)
        {
            if (cliente.FechaDeNacimiento.Year < 1920 || cliente.FechaDeNacimiento.Year > (DateTime.Today.Year - 12))
            {
                ModelState.AddModelError(nameof(cliente.FechaDeNacimiento), "Año de nacimiento inválido");
            }
        }

        private void ValidarEmailExistente(Cliente cliente)
        {
            if (_context.Clientes.Any(e => Comparar(e.Email, cliente.Email) && e.Id != cliente.Id))
            {
                ModelState.AddModelError(nameof(cliente.Email), "Ya existe un cliente con este Email");
            }
        }

        //Función que compara que los nombres no sean iguales, ignorando espacios y case. 
        private static bool Comparar(string s1, string s2)
        {
            return s1.Where(c => !char.IsWhiteSpace(c)).Select(char.ToUpperInvariant)
                .SequenceEqual(s2.Where(c => !char.IsWhiteSpace(c)).Select(char.ToUpperInvariant));
        }



    }
}