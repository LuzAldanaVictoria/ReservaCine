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
        [Authorize(Roles = nameof(Role.Administrador))]
        public async Task<IActionResult> Index()
        {
            var funciones = await _context
                .Funciones
                .Include(x => x.Pelicula)
                .Include(x => x.Sala)
                .ToListAsync();

            return View(funciones);
        }


        // GET: Funciones/Details/5
        [Authorize(Roles = nameof(Role.Administrador))]
        public async Task<IActionResult> Details(int? id)
        {
             
                if (id == null)
            {
                return NotFound();
            }


            var funcion = await _context.Funciones
                .Include(x => x.Reservas).ThenInclude(x => x.Cliente)
                .Include(x => x.Pelicula)
                .Include(x => x.Sala)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (funcion == null)
            {
                return NotFound();
            }


            return View(funcion);
        }

       
        // GET: Funciones/Create
        [Authorize(Roles = nameof(Role.Administrador))]
        public IActionResult Create()
        {
            ViewBag.SelectSalas = new SelectList(_context.Salas, "Id", "Nombre");
            ViewBag.SelectPeliculas = new SelectList(_context.Peliculas, "Id", "Nombre");
            
            return View();
        }

        // POST: Funciones/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = nameof(Role.Administrador))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Funcion funcion)
        {
            ValidarFecha(funcion);
            ValidarHorario(funcion);
            

            if (ModelState.IsValid)
            {
                var sala = await _context.Salas
                             .Where(x => x.Id == funcion.SalaId)
                             .FirstOrDefaultAsync();

                funcion.CantButacasDisponibles = sala.CapacidadTotal;
                _context.Add(funcion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.SelectSalas = new SelectList(_context.Salas, "Id", "Nombre");
            ViewBag.SelectPeliculas = new SelectList(_context.Peliculas, "Id", "Nombre");


            return View(funcion);
        }


        // GET: Funciones/Edit/5
        [Authorize(Roles = nameof(Role.Administrador))]
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

            ViewBag.SelectSalas = new SelectList(_context.Salas, "Id", "Nombre");
            ViewBag.SelectPeliculas = new SelectList(_context.Peliculas, "Id", "Nombre");

            return View(funcion);
        }

        // POST: Funciones/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = nameof(Role.Administrador))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Funcion funcion)
      
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
                    // Obtengo la función de la base de datos
                    var funcionDb = _context.Funciones.Find(id);

                    // Si la sala fue modificada, debemos alterar la cantidad de butacas disponibles de la función en base a la disponibilidad 
                    // de la nueva sala, pero teniendo en cuenta las butacas que ya habían sido reservadas.
                    if (funcionDb.SalaId != funcion.SalaId)
                    {
                        var salaAnterior = _context.Salas.Find(funcionDb.SalaId);
                        var salaNueva = _context.Salas.Find(funcion.SalaId);

                        var cantidadButacasReservadas = salaAnterior.CapacidadTotal - funcionDb.CantButacasDisponibles;
                        funcionDb.CantButacasDisponibles = salaNueva.CapacidadTotal - cantidadButacasReservadas;
                    }

                    // Mapeo los campos que se pueden editar SalaId, PeliculaId, Fecha, Horario
                    funcionDb.SalaId = funcion.SalaId;
                    funcionDb.PeliculaId = funcion.PeliculaId;
                    funcionDb.Fecha = funcion.Fecha;
                    funcionDb.Horario = funcion.Horario;

                    _context.Update(funcionDb);
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

            ViewBag.SelectSalas = new SelectList(_context.Salas, "Id", "Nombre");
            ViewBag.SelectPeliculas = new SelectList(_context.Peliculas, "Id", "Nombre");
            return View(funcion);
        }

        // GET: Funciones/Delete/5
        [Authorize(Roles = nameof(Role.Administrador))]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funcion = await _context
                .Funciones
                .Include(x => x.Pelicula)
                .Include(x => x.Sala)
                .FirstOrDefaultAsync(m => m.Id == id);
           
            if (funcion == null)
            {
                return NotFound();
            }


            return View(funcion);
        }

        // POST: Funciones/Delete/5
        [Authorize(Roles = nameof(Role.Administrador))]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var funcion = await _context.Funciones.FindAsync(id);
            _context.Funciones.Remove(funcion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult FiltrarPorPelicula(Pelicula pelicula) //cuando entra por cartelera
        {
            var funciones = _context
                 .Funciones
                 .Include(x => x.Pelicula)
                 .Include(x => x.Sala)
                 .ThenInclude(x => x.Tipo)
                 .Where(x => x.PeliculaId == pelicula.Id && x.Fecha >= DateTime.Now)
                 .ToList();

            return View(funciones);
        }

        [Authorize(Roles = nameof(Role.Cliente))]
        public IActionResult SeleccionarFiltro()
        {
            ViewBag.SelectPeliculas = new SelectList(_context.Peliculas, "Id", "Nombre");
            return View();
        }


        [Authorize(Roles = nameof(Role.Cliente))]
        public IActionResult FiltrarPorPeliculaId(int PeliculaId) // Cuando entra por el filtro día/pelicula
        {
            List<Funcion> funciones =
                _context.Funciones
                .Include(x => x.Pelicula)
                .Include(x => x.Sala)
                    .ThenInclude(x => x.Tipo)
                .Where(x => x.Pelicula.Id == PeliculaId && x.Fecha >= DateTime.Now)
                .ToList();

            return View(funciones);
        }


        [Authorize(Roles = nameof(Role.Cliente))]
        public IActionResult FiltrarPorFecha(DateTime Fecha) // Cuando entra por el filtro día/pelicula
        {

            List<Funcion> funciones =
                _context
                .Funciones
                .Include(x => x.Pelicula)
                .Include(x => x.Sala)
                    .ThenInclude(x => x.Tipo)
                .Where(x => x.Fecha == Fecha)
                .ToList();

            return View(funciones);
        }


        private bool FuncionExists(int id)
        {
            return _context.Funciones.Any(e => e.Id == id);
        }


        private void ValidarFecha(Funcion funcion)
        {
            if (funcion.Fecha < DateTime.Now) 
            {
                ModelState.AddModelError(nameof(funcion.Fecha), "La fecha no puede ser anterior a la fecha actual");
            }

            if(funcion.Fecha.Year > DateTime.Now.Year + 1)
            {
                ModelState.AddModelError(nameof(funcion.Fecha), "La fecha debe ser dentro del año actual");
            }
        }


        private void ValidarHorario(Funcion funcion)
        {
            if (funcion.Horario.Hour > 1 && funcion.Horario.Hour < 9)
            {
                ModelState.AddModelError(nameof(funcion.Horario), "El horario debe estar comprendido entre las 9:00 y la 01:59 (A.M.)");
            }

            ValidarSalaLibre(funcion);  // Si validar horario esta OK, llama a Validar sala libre

        }

      
        // Valida que la sala se encuentre libre cuando quiero crear o editar una funcion
        private void ValidarSalaLibre(Funcion f)  
        {
            
            if (_context.Funciones.Any(
                x => x.Fecha == f.Fecha &&
                x.SalaId == f.SalaId &&
               (x.Horario.Hour >= f.Horario.Hour - 3 && x.Horario.Hour <= f.Horario.Hour + 3) &&
                x.Id != f.Id))// verifico que el Id para que la funcion no se encuentre a si misma en caso de edicion
            {
                ModelState.AddModelError(nameof(f.Horario), "La sala está ocupada en ese horario");

            }




        }


    }

}
