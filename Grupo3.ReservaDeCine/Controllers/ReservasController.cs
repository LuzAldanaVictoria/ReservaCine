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

namespace Grupo3.ReservaDeCine.Controllers
{
    [Authorize]
    public class ReservasController : Controller
    {
        private readonly CineDbContext _context;

        public ReservasController(CineDbContext context)
        {
            _context = context;
        }

        // GET: Reservas
        public async Task<IActionResult> Index()
        {
            var reservas = await _context
                 .Reservas
                 .Include(x => x.Cliente)
                 .Include(x => x.Funcion).ThenInclude(x => x.Pelicula)
                 .ToListAsync();

            return View(reservas);
        }

        // GET: Reservas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas
                 .Include(x => x.Cliente)
                 .Include(x => x.Funcion).ThenInclude(x => x.Pelicula)
                 .FirstOrDefaultAsync(m => m.Id == id);

            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        [Authorize(Roles = nameof(Role.Cliente))]
        // GET: Reservas/Create
        public IActionResult Create()
        {
            ViewBag.SelectFechaHora = new SelectList(_context.Funciones, "Id", "FechaHora");

            return View();
        }

        [HttpGet]
        [Authorize(Roles = nameof(Role.Cliente))]
        public IActionResult MisReservas()
        {
            int clienteId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            List<Reserva> reservas = _context
                .Reservas
                .Include(x => x.Funcion).ThenInclude(x => x.Pelicula)
                .Where(reserva => reserva.ClienteId == clienteId && reserva.Funcion.Fecha >= DateTime.Today)
                .ToList();

            return View(reservas);
        }

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
                ModelState.AddModelError(string.Empty, "No cuenta con edad suficiente para ver esta Película.");
            }

            var funcion = _context
                .Funciones
                .Include(x => x.Sala).ThenInclude(x => x.Tipo)
                .FirstOrDefault(x => x.Id == reserva.FuncionId);


            if (funcion == null)
                ModelState.AddModelError(nameof(Reserva.Funcion), "La función no se encuentra disponible");

            ValidarCantButacas(reserva, funcion);

            if (ModelState.IsValid)
            {
                reserva.FechaDeAlta = DateTime.Now;
                funcion.CantButacasDisponibles -= reserva.CantButacas;
                reserva.CostoTotal = reserva.CantButacas * funcion.Sala.Tipo.PrecioEntrada;
                reserva.ClienteId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                _context.Add(reserva);
                _context.SaveChanges();
                return RedirectToAction(nameof(MisReservas));
            }

            ViewData["CostoEntrada"] = funcion.Sala.Tipo.PrecioEntrada;
            ViewData["Peliculas"] = new SelectList(_context.Peliculas, "Id", "Nombre", funcion.PeliculaId);
            ViewData["FechaHora"] = new SelectList(_context.Funciones, "Id", "FechaHora", funcion.FechaHora);

            reserva.Funcion = funcion;

            return View(reserva);
        }

        //POST: Reservas/Create
        //To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Reserva reserva)
        {
            if (!ValidarEdad(reserva))
            {
                ModelState.AddModelError(string.Empty, "No cuenta con edad suficiente para ver esta Película.");
            }

            var funcion = await _context.Funciones
            .Include(x => x.Sala).ThenInclude(x => x.Tipo)
            .Where(x => x.Id == reserva.FuncionId)
            .FirstOrDefaultAsync();

            if (funcion == null)
                ModelState.AddModelError(nameof(Reserva.Funcion), "La función no se encuentra disponible");

            ValidarCantButacas(reserva, funcion);


            if (ModelState.IsValid)
            {
                reserva.FechaDeAlta = DateTime.Now;
                funcion.CantButacasDisponibles -= reserva.CantButacas;
                reserva.CostoTotal = reserva.CantButacas * funcion.Sala.Tipo.PrecioEntrada;
                reserva.ClienteId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                _context.Add(reserva);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.SelectFunciones = new SelectList(_context.Funciones, "Id", "FechaHora");

            return View(reserva);
        }

        // GET: Reservas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null)
            {
                return NotFound();
            }
            ViewBag.SelectClientes = new SelectList(_context.Clientes, "Id", "Nombre");
            ViewBag.SelectFunciones = new SelectList(_context.Funciones, "Id", "Id");

            return View(reserva);
        }



        // GET: Reservas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context
                .Reservas
                .Include(x => x.Cliente)
                .Include(x => x.Funcion).ThenInclude(x => x.Pelicula)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        // POST: Reservas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            _context.Reservas.Remove(reserva);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservaExists(int id)
        {
            return _context.Reservas.Any(e => e.Id == id);

        }

        private void ValidarCantButacas(Reserva reserva, Funcion funcion)
        {
            if (funcion.CantButacasDisponibles < reserva.CantButacas)
            {
                ModelState.AddModelError(nameof(reserva.CantButacas), "No hay suficiente cantidad de butacas disponibles para esta función");
            }

            if (reserva.CantButacas > 10)
            {
                ModelState.AddModelError(nameof(reserva.CantButacas), "No es posible reservar más de 10 butacas.");
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
