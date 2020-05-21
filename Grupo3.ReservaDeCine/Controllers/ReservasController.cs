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
            await _context.Usuarios.ToListAsync();
            await _context.Peliculas.ToListAsync();
            await _context.Funciones.ToListAsync();
            await _context.Salas.ToListAsync();
            await _context.TiposSala.ToListAsync();
            return View(await _context.Reservas.ToListAsync());
        }

        // GET: Reservas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reserva == null)
            {
                return NotFound();
            }

            await _context.Usuarios.ToListAsync();
            await _context.Peliculas.ToListAsync();
            await _context.Funciones.ToListAsync();

            return View(reserva);
        }

        // GET: Reservas/Create
        public IActionResult Create()
        {
            ViewBag.TipoUsuarios = new SelectList(_context.Usuarios, "Id", "Email");
            // ViewBag.SelectPelicula = new SelectList(_context.Peliculas, "Id", "Nombre");
            ViewBag.SelectFunciones = new SelectList(_context.Funciones, "Id", "Id");
           // ViewBag.TipoFunciones = new SelectList(_context.Funciones, "Id", "Fecha");


            return View();
        }

        // POST: Reservas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id, Usuario, UsuarioId, FuncionId, Funcion" ,"CantButacas")] Reserva reserva)
        {



            //validacion
            //if (reserva.CantButacas > reserva.Funcion.CantButacasDisponibles)
            //{
            //    ModelState.AddModelError("CantButacas", "Las butacas seleccionadas superan la cantidad de butacas disponibles");
            //}

            if (ModelState.IsValid)
            {
                ////Define el precio de acuerdo al tipo de sala
          //     reserva.CostoTotal = reserva.CantButacas * reserva.Funcion.Sala.Tipo.PrecioEntrada; 
                reserva.FechaDeAlta = DateTime.Now;
                //reserva.Funcion.CantButacasDisponibles -= reserva.CantButacas;
                _context.Add(reserva);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.TipoUsuarios = new SelectList(_context.Usuarios, "Id", "Email");
            //ViewBag.SelectPelicula = new SelectList(_context.Peliculas, "Id", "Nombre");
            ViewBag.SelectFunciones = new SelectList(_context.Funciones, "Id", "Id");
           // ViewBag.TipoFunciones = new SelectList(_context.Funciones, "Id", "Fecha");
   
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
            ViewBag.TipoUsuarios = new SelectList(_context.Usuarios, "Id", "Nombre");
            //ViewBag.SelectPelicula = new SelectList(_context.Peliculas, "Id", "Nombre");
            ViewBag.SelectFunciones = new SelectList(_context.Funciones, "Id", "Id");
            ViewBag.TipoFunciones = new SelectList(_context.Funciones, "Id", "Fecha");
        
            return View(reserva);
        }

        // POST: Reservas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Usuario, UsuarioId, FuncionId, Funcion", "CantButacas")] Reserva reserva)
        {
            if (id != reserva.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
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
            ViewBag.TipoUsuarios = new SelectList(_context.Usuarios, "Id", "Nombre");
            // ViewBag.SelectPelicula = new SelectList(_context.Peliculas, "Id", "Nombre");
            ViewBag.SelectFunciones = new SelectList(_context.Funciones, "Id", "Id");
            ViewBag.TipoFunciones = new SelectList(_context.Funciones, "Id", "Fecha");

            return View(reserva);
        }

        // GET: Reservas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reserva == null)
            {
                return NotFound();
            }

            await _context.Usuarios.ToListAsync();
            await _context.Peliculas.ToListAsync();
            await _context.Funciones.ToListAsync();
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
    }
}
