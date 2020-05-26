using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Grupo3.ReservaDeCine.Database;
using Grupo3.ReservaDeCine.Models;

namespace Grupo3.ReservaDeCine.Controllers
{

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

        // GET: Reservas/Create
        public IActionResult Create()
        {
            ViewBag.SelectClientes = new SelectList(_context.Clientes, "Id", "Email");
            ViewBag.SelectFunciones = new SelectList(_context.Funciones, "Id", "Id");

            return View();
        }

        // POST: Reservas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id, Cliente, ClienteId, FuncionId, Funcion" ,"CantButacas")] Reserva reserva)
        {
            var funcion = await _context.Funciones
            .Where(x => x.Id == reserva.FuncionId)
            .FirstOrDefaultAsync();

            var sala = await _context.Salas
            .Where(x => x.Id == funcion.SalaId)
            .FirstOrDefaultAsync();

            var tipoSala = await _context.TiposSala
            .Where(x => x.Id == sala.TipoId)
            .FirstOrDefaultAsync();

            ValidarCantButacas(reserva, funcion);
          

            if (ModelState.IsValid)
            {
                reserva.FechaDeAlta = DateTime.Now;
                funcion.CantButacasDisponibles -= reserva.CantButacas;
                reserva.CostoTotal = reserva.CantButacas * tipoSala.PrecioEntrada;
                _context.Add(reserva);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.SelectClientes = new SelectList(_context.Clientes, "Id", "Email");
            ViewBag.SelectFunciones = new SelectList(_context.Funciones, "Id", "Id");
   
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
        public async Task<IActionResult> Edit(int id,  Reserva reserva)
        {

            var funcion = await _context.Funciones
            .Where(x => x.Id == reserva.FuncionId)
            .FirstOrDefaultAsync();

            var sala = await _context.Salas
            .Where(x => x.Id == funcion.SalaId)
            .FirstOrDefaultAsync();

            var tipoSala = await _context.TiposSala
            .Where(x => x.Id == sala.TipoId)
            .FirstOrDefaultAsync();

            var Cliente = await _context.Clientes
            .Where(x => x.Id == reserva.ClienteId)
            .FirstOrDefaultAsync();


            if (id != reserva.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                funcion.CantButacasDisponibles -= reserva.CantButacas;
                reserva.CostoTotal = reserva.CantButacas * tipoSala.PrecioEntrada;
                try
                {
                    _context.Update(reserva);
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

       private void ValidarCantButacas (Reserva reserva, Funcion funcion)
        {
            if (funcion.CantButacasDisponibles < reserva.CantButacas)
            {
                ModelState.AddModelError(nameof(reserva.CantButacas), "No hay suficiente cantidad de butacas disponibles para esta función");
            }

            if (funcion.CantButacasDisponibles > 10)
            {
                ModelState.AddModelError(nameof(reserva.CantButacas), "No es posible reservar más de 10 butacas.");
            }
        }
    }
}
