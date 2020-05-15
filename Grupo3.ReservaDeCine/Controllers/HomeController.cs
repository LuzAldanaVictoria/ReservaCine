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
            

                _context.AddRange(new[] { sala1, sala2, sala3, sala4 });

                _context.SaveChanges();
            }
        }
    }
}
