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
    public class FuncionesController : Controller
    {
        private readonly CineDbContext _context;

        public FuncionesController(CineDbContext context)
        {
            _context = context;
        }

        // GET: Funciones
        public async Task<IActionResult> Index()
        {
            //esta linea hace que se carguen los nombres de las peliculas
            await _context.Peliculas.ToListAsync();

            //esta linea hace que se carguen los nombres de las salas
            await _context.Salas.ToListAsync();

            return View(await _context.Funciones.ToListAsync());
        }

        // GET: Funciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
             
                if (id == null)
            {
                return NotFound();
            }

            var funcion = await _context.Funciones
                .Include(x => x.Reservas).ThenInclude(x => x.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (funcion == null)
            {
                return NotFound();
            }

            await _context.Peliculas.ToListAsync();
            await _context.Salas.ToListAsync();
            await _context.Reservas.ToListAsync();

            return View(funcion);
        }



        // GET: Funciones/Create
        public IActionResult Create()
        {
            ViewBag.Salas = new SelectList(_context.Salas, "Id", "Nombre");
            ViewBag.Peliculas = new SelectList(_context.Peliculas, "Id", "Nombre");
           

            return View();
        }

        // POST: Funciones/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id, SalaId, Sala, PeliculaId, Pelicula, Fecha, Horario, CantButacasDisponibles, CapacidadTotal ")]Funcion funcion)
        {
           var sala = await _context.Salas
           .Where(x => x.Id == funcion.SalaId)
           .FirstOrDefaultAsync();

            ValidarFecha(funcion);
            ValidarHorario(funcion);
            

            if (ModelState.IsValid)
            {
                funcion.CantButacasDisponibles = sala.CapacidadTotal;
                _context.Add(funcion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Salas = new SelectList(_context.Salas, "Id", "Nombre");
            ViewBag.Peliculas = new SelectList(_context.Peliculas, "Id", "Nombre");


            return View(funcion);
        }

        // GET: Funciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funcion = await _context.Funciones.FindAsync(id);
            if (funcion == null)
            {
                return NotFound();
            }

            ValidarFecha(funcion);
            ValidarHorario(funcion);


            ViewBag.Salas = new SelectList(_context.Salas, "Id", "Nombre");
            ViewBag.Peliculas = new SelectList(_context.Peliculas, "Id", "Nombre");
            return View(funcion);
        }

        // POST: Funciones/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SalaId, Sala, PeliculaId, Pelicula, Fecha, Horario, CantButacasDisponibles, ")] Funcion funcion)
        {
            
            if (id != funcion.Id)
            {
                return NotFound();
            }
            
            ValidarFecha(funcion);
            ValidarHorario(funcion);


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(funcion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FuncionExists(funcion.Id))
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

            ViewBag.Salas = new SelectList(_context.Salas, "Id", "Nombre");
            ViewBag.Peliculas = new SelectList(_context.Peliculas, "Id", "Nombre");
            return View(funcion);
        }

        // GET: Funciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funcion = await _context.Funciones
                .FirstOrDefaultAsync(m => m.Id == id);
            if (funcion == null)
            {
                return NotFound();
            }

            await _context.Peliculas.ToListAsync();
            await _context.Salas.ToListAsync();
            return View(funcion);
        }

        // POST: Funciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var funcion = await _context.Funciones.FindAsync(id);
            _context.Funciones.Remove(funcion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FuncionExists(int id)
        {
            return _context.Funciones.Any(e => e.Id == id);
        }

        private void ValidarFecha(Funcion funcion)
        {
            if (funcion.Fecha < DateTime.Now || funcion.Fecha.Year > DateTime.Now.Year + 1)
            {
                ModelState.AddModelError(nameof(funcion.Fecha), "Fecha inválida");
            }
        }

        private void ValidarHorario(Funcion funcion)
        {
            if (funcion.Horario.Hour > 1 && funcion.Horario.Hour < 9)
            {
                ModelState.AddModelError(nameof(funcion.Horario), "El horario debe estar comprendido entre las 9:00 y la 01:59 (A.M.)");
            }
        }



    }

}
