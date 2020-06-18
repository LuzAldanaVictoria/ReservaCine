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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id, SalaId, Sala, PeliculaId, Pelicula, Fecha, Horario, CantButacasDisponibles, CapacidadTotal")]Funcion funcion)
        {
           var sala = await _context.Salas
           .Where(x => x.Id == funcion.SalaId)
           .FirstOrDefaultAsync();

            ValidarFecha(funcion);
            ValidarHorario(funcion);
            ValidarSalaLibre(funcion);



            if (ModelState.IsValid)
            {
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

            ValidarFecha(funcion);
            ValidarHorario(funcion);


            ViewBag.SelectSalas = new SelectList(_context.Salas, "Id", "Nombre");
            ViewBag.SelectPeliculas = new SelectList(_context.Peliculas, "Id", "Nombre");
            return View(funcion);
        }

        // POST: Funciones/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
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
        //[Authorize(Roles = nameof(Role.Administrador))]
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

        public IActionResult FiltrarPorPelicula(Pelicula pelicula)
        {
            var funciones = _context
                .Funciones
                .Include(x => x.Pelicula)
                .Include(x => x.Sala)
                .ThenInclude(x => x.Tipo)
                .Where(x => x.Pelicula == pelicula && x.Fecha >= DateTime.Now)
                .ToList();

          

            return View(funciones);
        }


        public IActionResult FiltrarPorPeliculaId(int? PeliculaId)
        {
            var funciones = _context
                .Funciones
                .Include(x => x.Pelicula)
                .Include(x => x.Sala)
                .ThenInclude(x => x.Tipo)
                .Where(x => x.Pelicula.Id == PeliculaId && x.Fecha >= DateTime.Now)
                .ToList();


            return View(funciones);
        }


        public IActionResult FiltrarPorDia(DateTime dia)
        {
           
            var funciones = _context
                .Funciones
                .Include(x => x.Pelicula)
                .Include(x => x.Sala)
                .ThenInclude(x => x.Tipo)
                .Where(x => x.Fecha.ToString("dd/MM/yyyy") == dia.ToString("dd/MM/yyyy"))
                .ToList();

            return View(funciones);
        }


         public IActionResult SeleccionarFiltro(int PeliculaId, DateTime fecha)
        
        {
            ViewBag.SelectPeliculas = new SelectList(_context.Peliculas, "Id", "Nombre");

            List<Funcion> funcionesPelicula = _context
                                               .Funciones
                                               .Include(x => x.Pelicula)
                                               .Where(x => x.PeliculaId == PeliculaId)
                                               .ToList();

            List<Funcion> funcionesFecha = _context
                                            .Funciones
                                            .Where(x => x.Fecha == fecha)
                                            .ToList();

            return View();

        }

        // No se permite crear una funcion en la misma sala, en un rango horario de 3 horas aprox de lo que duraria la pelicula
        //ToDo: Faltaria la comparacion con minutos
        private void ValidarSalaLibre(Funcion f)
        {
            var funciones = _context.Funciones
               .Where(x => x.Fecha == f.Fecha && 
                            (x.Horario.Hour >= f.Horario.Hour -3  && x.Horario.Hour <= f.Horario.Hour +3 ) &&
                            x.SalaId== f.SalaId)
                .ToList();
           

             if (funciones.Count >0)
            {
                ModelState.AddModelError(nameof(f.Horario), "La sala está ocupada en ese horario");
            }

            
        }



    }

}
