using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Grupo3.ReservaDeCine.Models;
using Grupo3.ReservaDeCine.Database;
using Microsoft.AspNetCore.Authorization;
using Grupo3.ReservaDeCine.Models.Enums;
using Grupo3.ReservaDeCine.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Grupo3.ReservaDeCine.Controllers
{
    public class HomeController : Controller
    {

        private readonly CineDbContext _context;

        public HomeController(CineDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var primerLogin = TempData["primerLogin"] as bool?;
            ViewBag.PrimerLogin = primerLogin ?? false;

            Seed();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private void Seed()
        {
            if (!_context.Funciones.Any())
            {

                var sala1 = new Sala()
                {
                    Nombre = "Sala 1",
                    Tipo = new TipoSala()
                    {
                        Nombre = "Premium",
                        PrecioEntrada = 550
                    },
                    CapacidadTotal = 10
                };

                var sala2 = new Sala()
                {
                    Nombre = "Sala 2",
                    Tipo = new TipoSala()
                    {
                        Nombre = "2D",
                        PrecioEntrada = 250
                    },
                    CapacidadTotal = 120
                };

                var sala3 = new Sala()
                {
                    Nombre = "Sala 3",
                    Tipo = new TipoSala()
                    {
                        Nombre = "3D",
                        PrecioEntrada = 350
                    },
                    CapacidadTotal = 100
                };

                var sala4 = new Sala()
                {
                    Nombre = "Sala 4",
                    Tipo = new TipoSala()
                    {
                        Nombre = "4D",
                        PrecioEntrada = 450
                    },
                    CapacidadTotal = 80
                };

                var Cliente1 = new Cliente()
                {
                    Role = Role.Cliente,
                    Username ="cliente1",
                    Password = "1234".Encriptar(),
                    Nombre = "Luciano",
                    Apellido = "García",
                    Email = "lucianogarcia@gmail.com",
                    FechaDeNacimiento = new DateTime(1992, 06, 20),
                    Reservas = new List<Reserva>()
                    {
                        new Reserva()
                        {
                            Funcion = new Funcion()
                            {
                                Sala = sala1,
                                Pelicula = new Pelicula()
                                {
                                    Nombre = "La Bella y La Bestia",
                                    Genero = new Genero
                                    {
                                        Descripcion = "Infantil"
                                    },
                                    Sinopsis = "Bella es una brillante y guapa joven que utiliza la lectura como válvula de escape de su rutinaria vida. Cuando su padre es apresado en un misterioso castillo, " +
                                    "Bella acude en su búsqueda y se presta a ocupar su lugar. El castillo es propiedad de una atormentada Bestia que, como Bella comprobará con el tiempo, resulta ser un joven príncipe " +
                                    "bajo los efectos de un hechizo. Sólo cuando conozca el amor, el príncipe podrá volver a su verdadero cuerpo.",
                                    Clasificacion = new Clasificacion
                                    {
                                        Descripcion = "ATP",
                                        EdadMinima = 0
                                    },
                                },
                                Fecha = new DateTime(2020,07,15),
                                Horario = new DateTime().AddHours(14).AddMinutes(00),
                                CantButacasDisponibles = sala1.CapacidadTotal
                            },
                            CantButacas = 2,
                            CostoTotal = 1100,
                            FechaDeAlta = DateTime.Now

                        },

                         new Reserva()
                        {
                            Funcion = new Funcion()
                            {
                                Sala = sala2,
                                Pelicula = new Pelicula()
                                {
                                    Nombre = "Matrix",
                                    Genero = new Genero
                                    {
                                        Descripcion = "Ciencia ficción"
                                    },
                                    Sinopsis = "Neo es un joven pirata informático que lleva una doble vida: durante el día ejerce en una empresa de servicios informáticos, mientras que por la noche se dedica a piratear " +
                                    "bases de datos y saltarse sistemas de alta seguridad. Su vida cambiará cuando una noche conozca a Trinity, una misteriosa joven que parece ser una leyenda en el mundo de los 'hackers' informáticos," +
                                    " que lo llevará a Neo ante su líder: Morfeo. Así descubrirá una terrible realidad y el joven deberá decidir si unirse a la resistencia o vivir su vida como hasta ahora.",
                                    Clasificacion = new Clasificacion
                                    {
                                        Descripcion = "+16",
                                        EdadMinima = 16
                                    },
                                },
                                Fecha = new DateTime(2020,04,17),
                                Horario = new DateTime().AddHours(20).AddMinutes(20),
                                CantButacasDisponibles = sala3.CapacidadTotal

                            },
                            CantButacas = 4,
                            CostoTotal = 1800,
                            FechaDeAlta = DateTime.Now
                        },

                    },
                    FechaDeAlta = DateTime.Now
                };


                var Cliente2 = new Cliente()
                {
                    Role = Role.Cliente,
                    Username = "cliente2",
                    Password = "1234".Encriptar(),
                    Nombre = "Carlos",
                    Apellido = "Pereyra",
                    Email = "cp2020@gmail.com",
                    FechaDeNacimiento = new DateTime(2000, 11, 04),
                    FechaDeAlta = DateTime.Now
                };

                var Cliente3 = new Cliente()
                {
                    Role = Role.Cliente,
                    Username = "cliente3",
                    Password = "1234".Encriptar(),
                    Nombre = "Carla",
                    Apellido = "Rodriguez",
                    Email = "carla@gmail.com",
                    FechaDeNacimiento = new DateTime(1987, 01, 03),
                    FechaDeAlta = DateTime.Now
                };


                var administrador1 = new Administrador()
                {
                    Role = Role.Administrador,
                    Username = "administrador1",
                    Password = "1234".Encriptar(),
                    Nombre = "Juan Carlos",
                    Apellido = "Gonzalez",
                    Email = "jcgonzalez@gmail.com",
                    Legajo = 0001 
                };


                _context.AddRange(new[] { Cliente1, Cliente2, Cliente3 });
                _context.Add(administrador1);
                _context.AddRange(new[] { sala1, sala2, sala3, sala4 });
                _context.SaveChanges();

                }
            }
        }


    

    }



