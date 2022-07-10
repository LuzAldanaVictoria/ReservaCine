using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Grupo3.ReservaDeCine.Database;
using Grupo3.ReservaDeCine.Models;
using Microsoft.AspNetCore.Authorization;
using Grupo3.ReservaDeCine.Models.Enums;
using System.Security.Claims;
using Microsoft.Extensions.Logging.Console.Internal;
using System.Security.Cryptography.X509Certificates;

namespace Grupo3.ReservaDeCine.Controllers
{
    [Authorize] // Solo pueden acceder personas autenticadas

    public class ReservasController : Controller
    {
        private readonly CineDbContext _context; // Es muy importante que sea solo “readonly”


        //esto es una inyeccion de dependencia, cuando se contruya la instancia de ReservasController,
        //voy a inyectarle el contexto de BD
        public ReservasController(CineDbContext context) 
        {
            _context = context;// recibo el contexto y lo guardo en esta variable, para poder interactual con la BD
        }

        [HttpGet]
        [Authorize(Roles = nameof(Role.Administrador))]
        public IActionResult Index() // el IactionResult, es una “interfaz”
        {
            var reservas = _context //Pido el contexto de reserva,“accedo al set de datos de reservas”
                 .Reservas
                 .Include(x => x.Cliente) //Pido los clientes de reserva
                 .Include(x => x.Funcion).ThenInclude(x => x.Pelicula) // Pido las funciones y que de las funciones me traiga las peliculas
                 .Include(x => x.Funcion).ThenInclude(x => x.Sala) // / De la funcion, pido que me inluya la sala
                 .ToList(); // Me lo presenta en forma de lista

            return View(reservas); // Le mando a la vista la lista de reservas
        }


        [HttpGet]
        [Authorize(Roles = nameof(Role.Administrador))]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = _context.Reservas
                 .Include(x => x.Cliente)
                 .Include(x => x.Funcion).ThenInclude(x => x.Pelicula)
                 .FirstOrDefault(x => x.Id == id);

            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }


        [HttpGet]
        [Authorize(Roles = nameof(Role.Cliente))]
        public IActionResult MisReservas()
        {
            int clienteId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value); // levante el id del usuario que esta autenticado.

            // la pantalla de mis reservas necesita la lista de mis reservas, que se hace asi:
            var reservas = _context
                .Reservas
                .Include(x => x.Funcion).ThenInclude(x => x.Pelicula)
                .Include(x => x.Funcion).ThenInclude(x => x.Sala)
                .Where(reserva => reserva.ClienteId == clienteId && reserva.Funcion.Fecha >= DateTime.Today) 
                .ToList();

            return View(reservas); // retorna a la vista y le pasa la lista de mis reservas
        }



        
        // Entrando por cartelera, seleccionando pelicula y luego reservar para una funcion determinada
        [HttpGet]
        [Authorize(Roles = nameof(Role.Cliente))]
        public IActionResult CrearReservaPorFuncion(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funcion = _context
                .Funciones
                .Include(x => x.Sala).ThenInclude(x => x.Tipo)
                .FirstOrDefault(x => x.Id == id);

            ViewData["Peliculas"] = new SelectList(_context.Peliculas, "Id", "Nombre", funcion.PeliculaId);
            ViewData["FechaHora"] = new SelectList(_context.Funciones, "Id", "FechaHora", funcion.FechaHora);
            ViewData["CostoEntrada"] = funcion.Sala.Tipo.PrecioEntrada;

            Reserva reserva = new Reserva()
            {
                FuncionId = funcion.Id,
                Funcion = funcion

            };

            return View(reserva);
        }

        [HttpPost]
        [Authorize(Roles = nameof(Role.Cliente))]
        // Aca el server efectiviza la reserva
        public IActionResult CrearReservaPorFuncion([Bind("FuncionId, CantButacas")] Reserva reserva)
        {
            if (!ValidarEdad(reserva))
            {
                ModelState.AddModelError(string.Empty, "No cuenta con edad suficiente para ver esta película");
            }

            var funcion = _context
                .Funciones
                .Include(x => x.Sala).ThenInclude(x => x.Tipo)
                .FirstOrDefault(x => x.Id == reserva.FuncionId);


            if (funcion == null)
                ModelState.AddModelError(nameof(Reserva.Funcion), "La función no se encuentra disponible");

            ValidarCantButacas(reserva, funcion); // función que verifica si me alcanzan la cantidad de butacas disponibles para hacer la reserva y que no sea mayor a 10

            if (ModelState.IsValid)
            {
                reserva.FechaDeAlta = DateTime.Now; // setea la fecha de alta de reserva al día de hoy
                funcion.CantButacasDisponibles -= reserva.CantButacas; // resta de las disponibles, las bustacas de la reserva
                reserva.CostoTotal = reserva.CantButacas * funcion.Sala.Tipo.PrecioEntrada; ; // calcula el costo de la reserva
                reserva.ClienteId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);  // aca le asigna a la reserva el clienteId con el claims de Name Identifier
                _context.Add(reserva); // agrega la reserva a la base de datos
                _context.SaveChanges();  //guardo cambios en db
                return RedirectToAction(nameof(MisReservas));// Si el modelo es valido, me redirige a MIS RESERVAS
            }


            //Si el modelo no es valido ?
            ViewData["CostoEntrada"] = funcion.Sala.Tipo.PrecioEntrada; //De las funciones pido que me haga una lista mostrando el ID y la FechaHora
            ViewData["Peliculas"] = new SelectList(_context.Peliculas, "Id", "Nombre", funcion.PeliculaId); //aca hago un view data, que después voy a usar en la vista para saber el nombre de la película
            ViewData["FechaHora"] = new SelectList(_context.Funciones, "Id", "FechaHora", funcion.FechaHora); //aca hago un view data, que después voy a usar en la vista para saber la fecha y hora de las funcions

            reserva.Funcion = funcion;

            return View(reserva);
        }


        [HttpGet]
        [Authorize(Roles = nameof(Role.Administrador))]
        public  IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva =  _context
                .Reservas
                .Include(x => x.Cliente)
                .Include(x => x.Funcion).ThenInclude(x => x.Pelicula)
                .FirstOrDefault(m => m.Id == id);

            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken] //###  //verificar que es esta linea
        [Authorize(Roles = nameof(Role.Administrador))]
        public  IActionResult DeleteConfirmed(int id)
        {
            var reserva =  _context.Reservas.Find(id);
           
            _context.Reservas.Remove(reserva);
            _context.SaveChanges();
            
            return RedirectToAction(nameof(Index));
        }



        ////---- Métodos privados para validaciones ----////

        private bool ReservaExists(int id)
        {
            return _context.Reservas.Any(x => x.Id == id);

        }

        private void ValidarCantButacas(Reserva reserva, Funcion funcion)
        {
            if (funcion.CantButacasDisponibles < reserva.CantButacas)
            {
                ModelState.AddModelError(nameof(reserva.CantButacas), "No hay suficiente cantidad de butacas disponibles para esta función");
            }
        }

        private bool ValidarEdad(Reserva reserva)
        {
            bool puede;

            var funcion = _context.Funciones
                           .Include(x => x.Pelicula).ThenInclude(x => x.Clasificacion)
                           .Where(x => x.Id == reserva.FuncionId)
                           .FirstOrDefault();

            var cliente = _context.Clientes.Find(int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value));
            var fechaNacimiento = cliente.FechaDeNacimiento;
            var fechaActual = DateTime.Now;
            int edad = fechaActual.Year - cliente.FechaDeNacimiento.Year;

            if (fechaActual.Month < fechaNacimiento.Month || (fechaActual.Month == fechaNacimiento.Month && fechaActual.Day < fechaNacimiento.Day))
            {
                edad--;
            }

            if (edad < funcion.Pelicula.Clasificacion.EdadMinima)
            {
                puede = false;
            }
            else
            {
                puede = true;
            }

            return puede;

        }

    }
}
