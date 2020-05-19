using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Grupo3.ReservaDeCine.Models;
using Grupo3.ReservaDeCine.Database;

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
                    CapacidadTotal = 50
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

                var usuario1 = new Usuario() {
                    Nombre = "Luciano",
                    Apellido = "García",
                    Email = "lucianogarcia@gmail.com",
                    FechaDeNacimiento = new DateTime(1992,06,20),
                    Reservas = new List<Reserva>()
                    {
                        new Reserva()
                        {
                            Funcion = new Funcion()
                            {
                                Sala = sala1,
                                Pelicula = new Pelicula()
                                {
                                    Nombre = "Beauty and the Beast",
                                    Genero = new Genero
                                    {
                                        Descripcion = "Infantil"
                                    },
                                    Sinopsis = "A selfish Prince is cursed to become a monster for the rest of his life, unless he learns to fall in love with a beautiful young woman he keeps prisoner.",
                                    Clasificacion = new Clasificacion
                                    {
                                        Descripcion = "ATP",
                                        EdadMinima = 0
                                    },
                                },
                                Fecha = new DateTime(2020,06,15),
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
                                Sala = sala3,
                                Pelicula = new Pelicula()
                                {
                                    Nombre = "The Matrix",
                                    Genero = new Genero
                                    {
                                        Descripcion = "Ciencia ficción"
                                    },
                                    Sinopsis = "A computer hacker learns from mysterious rebels about the true nature of his reality and his role in the war against its controllers.",
                                    Clasificacion = new Clasificacion
                                    {
                                        Descripcion = "+13",
                                        EdadMinima = 13
                                    },
                                },
                                Fecha = new DateTime(2020,06,17),
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


                var usuario2 = new Usuario()
                {
                    Nombre = "Carlos",
                    Apellido = "Pereyra",
                    Email = "cp2020@gmail.com",
                    FechaDeNacimiento = new DateTime(2000,11,04),
                    FechaDeAlta = DateTime.Now
                };

                var usuario3 = new Usuario()
                {
                    Nombre = "Carla",
                    Apellido = "Rodriguez",
                    Email = "carla@gmail.com",
                    FechaDeNacimiento = new DateTime(1987,01,03),
                    FechaDeAlta = DateTime.Now
                };

                _context.AddRange(new[] { usuario1, usuario2, usuario3 });
                _context.AddRange(new[] { sala1, sala2, sala3, sala4 });


                _context.SaveChanges();
            }
        }
    }
}
