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
using Remotion.Linq.Clauses;
using Microsoft.EntityFrameworkCore.Internal;

namespace Grupo3.ReservaDeCine.Controllers
{
    public class PeliculasController : Controller
    {
        private readonly CineDbContext _context;

        public PeliculasController(CineDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = nameof(Role.Administrador))]
        public IActionResult Index()
        {
            var peliculas = _context
               .Peliculas
               .Include(x => x.Generos)
               .Include(x => x.Clasificacion)
               .ToList();

            return View(peliculas);
        }

        // GET: Peliculas/Details/5
        [Authorize(Roles = nameof(Role.Administrador))]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pelicula = _context.Peliculas
                .Include(x => x.Generos).ThenInclude (x => x.Genero)
                .Include(x => x.Clasificacion)
                .Include(x => x.Funciones).ThenInclude (x => x.Sala)
                .FirstOrDefault(m => m.Id == id);

            if (pelicula == null)
            {
                return NotFound();
            }

            return View(pelicula);
        }

        [Authorize(Roles = nameof(Role.Administrador))]
        // GET: Peliculas/Create
        public IActionResult Create()
        {
            ViewBag.SelectGeneros = new MultiSelectList(_context.Generos, nameof(Genero.Id), nameof(Genero.Descripcion));
            ViewBag.SelectClasificaciones = new SelectList(_context.Clasificaciones, "Id", "Descripcion");

            return View();
        }

        // POST: Peliculas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = nameof(Role.Administrador))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Pelicula pelicula, List<int> generoIds)
        {

            //valida si ya existe el nombre
            ValidarNombreExistente(pelicula);
            ValidarGeneros(generoIds);

            if (ModelState.IsValid)
            {
                pelicula.Generos = new List<PeliculaGenero>();

                foreach (var generoId in generoIds)
                {
                    pelicula.Generos.Add(new PeliculaGenero { Pelicula = pelicula, GeneroId = generoId });
                }

                _context.Add(pelicula);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.SelectGeneros = new MultiSelectList(_context.Generos, nameof(Genero.Id), nameof(Genero.Descripcion), generoIds);
            ViewBag.SelectClasificaciones = new SelectList(_context.Clasificaciones, "Id", "Descripcion", pelicula.ClasificacionId);
            return View(pelicula);
        }


        [Authorize(Roles = nameof(Role.Administrador))]
        // GET: Peliculas/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pelicula = _context.Peliculas
                             .Include(x => x.Generos)
                             .FirstOrDefault(x => x.Id == id);

            if (pelicula == null)
            {
                return NotFound();
            }

            ViewBag.SelectGeneros = new MultiSelectList(_context.Generos, nameof(Genero.Id), nameof(Genero.Descripcion), pelicula.Generos.Select(x => x.GeneroId).ToList());
            ViewBag.SelectClasificaciones = new SelectList(_context.Clasificaciones, "Id", "Descripcion");
           
            return View(pelicula);
        }




        // POST: Peliculas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = nameof(Role.Administrador))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Pelicula pelicula, List<int> generoIds)
        {
            if (id != pelicula.Id)
            {
                return NotFound();
            }

            //valida si ya existe el nombre
            ValidarNombreExistente(pelicula);
            ValidarGeneros(generoIds);

            if (ModelState.IsValid)
            {
                try
                {
                    var peliculaDb = _context
                        .Peliculas
                        .Include(x => x.Generos)
                        .FirstOrDefault(x => x.Id == id);

                    peliculaDb.Nombre = pelicula.Nombre;
                    peliculaDb.ClasificacionId = pelicula.ClasificacionId;
                    peliculaDb.Sinopsis = pelicula.Sinopsis;
                   
                    
                    foreach (var peliculaGenero in peliculaDb.Generos)
                    {
                        _context.Remove(peliculaGenero);
                    }

                    foreach (var generoId in generoIds)
                    {
                        peliculaDb.Generos.Add(new PeliculaGenero { PeliculaId = peliculaDb.Id, GeneroId = generoId });
                    }


                    _context.Update(peliculaDb);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PeliculaExists(pelicula.Id))
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
           
            ViewBag.SelectGeneros= new MultiSelectList(_context.Generos, nameof(Genero.Id), nameof(Genero.Descripcion));
            ViewBag.SelectClasificaciones = new SelectList(_context.Clasificaciones, "Id", "Descripcion");

            return View(pelicula);
        }


        // GET: Peliculas/Delete/5
        [Authorize(Roles = nameof(Role.Administrador))]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pelicula = _context
                .Peliculas
                .Include(x => x.Generos)
                .Include(x => x.Clasificacion)
                .FirstOrDefault(m => m.Id == id);

            
            if (pelicula == null)
            {
                return NotFound();
            }

            return View(pelicula);
        }

        // POST: Peliculas/Delete/5
        [Authorize(Roles = nameof(Role.Administrador))]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pelicula = await _context.Peliculas.FindAsync(id);
            _context.Peliculas.Remove(pelicula);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Cartelera()
        {
            var cartelera = await _context
               .Peliculas
               .Include(x => x.Generos)
               .Include(x => x.Clasificacion)
               .Include(x => x.Funciones)
               .ToListAsync();

            return View(cartelera);
        }

        private bool PeliculaExists(int id)
        {
            return _context.Peliculas.Any(e => e.Id == id);
        }


        private void ValidarNombreExistente(Pelicula pelicula)
        {
            if (_context.Peliculas.Any(e => Comparar(e.Nombre, pelicula.Nombre) && e.Id != pelicula.Id))
            {
                ModelState.AddModelError(nameof(pelicula.Nombre), "Ya existe una película con ese nombre");
            }
        }


        //Función que compara que los nombres no sean iguales, ignorando espacios y case. 
        private static bool Comparar(string s1, string s2)
        {
            return s1.Where(c => !char.IsWhiteSpace(c)).Select(char.ToUpperInvariant)
                .SequenceEqual(s2.Where(c => !char.IsWhiteSpace(c)).Select(char.ToUpperInvariant));
        }

        private void ValidarGeneros(List<int> generoIds)
        {
            if (generoIds.Count == 0)
            {
                ModelState.AddModelError(nameof(Pelicula.Generos), "La pelicula debe tener al menos un género.");
            }
        }
    }
}
