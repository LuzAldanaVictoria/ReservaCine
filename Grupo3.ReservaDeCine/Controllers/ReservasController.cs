﻿using System;
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
                .Where(reserva => reserva.ClienteId == clienteId)
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
                .FirstOrDefault(x => x.Id == id);

            ViewData["Peliculas"] = new SelectList(_context.Peliculas, "Id", "Nombre", funcion.PeliculaId);

            Reserva reserva = new Reserva()
            {
                FuncionId = funcion.Id,
                Funcion = funcion
            };

            return View(reserva);
        }

        [HttpPost]
        [Authorize(Roles = nameof(Role.Cliente))]
        public IActionResult CrearReservaPorFuncion([Bind("FuncionId, CantButacas")] Reserva reserva)
        {
            var funcion = _context.Funciones
            .Include(x => x.Sala).ThenInclude(x => x.Tipo)
            .Where(x => x.Id == reserva.FuncionId)
            .FirstOrDefault();

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

            ViewData["Peliculas"] = new SelectList(_context.Peliculas, "Id", "Nombre", funcion.PeliculaId);

            reserva.Funcion = funcion;

            return View(reserva);
        }

        // POST: Reservas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id, Cliente, ClienteId, FuncionId, Funcion", "CantButacas")] Reserva reserva)
        {

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
                //no guarda el usuario 
                reserva.Cliente = User.Identity as Cliente;
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

        // POST: Reservas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Reserva reserva)
        {

            var funcion = await _context.Funciones
            .Include(x => x.Sala).ThenInclude(x => x.Tipo)
            .Where(x => x.Id == reserva.FuncionId)
            .FirstOrDefaultAsync();

            if (funcion == null)
                ModelState.AddModelError(nameof(Reserva.Funcion), "La función no se encuentra disponible");


            if (id != reserva.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                funcion.CantButacasDisponibles -= reserva.CantButacas;
                reserva.CostoTotal = reserva.CantButacas * funcion.Sala.Tipo.PrecioEntrada;
                try
                {
                    var reservaDb = _context.Reservas.Find(id);
                    reserva.FechaDeAlta = reservaDb.FechaDeAlta;
                    _context.Update(reservaDb);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservaExists(reserva.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
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
    }
}
