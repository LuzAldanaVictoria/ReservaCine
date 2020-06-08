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
    public class PeliculasController : Controller
    {
        private readonly CineDbContext _context;

        public PeliculasController(CineDbContext context)
        {
            _context = context;
        }

  
      
        
        public async Task<IActionResult> Index()
        {
            var peliculas = await _context
               .Peliculas
               .Include(x => x.Genero)
               .Include(x => x.Clasificacion)
               .ToListAsync();

            return View(peliculas);
        }

        // GET: Peliculas/Details/5
        public async Task<IActionResult> Details(int? id)
        {


            if (id == null)
            {
                return NotFound();
            }

            var pelicula = await _context.Peliculas
                .Include(x => x.Genero)
                .Include(x => x.Clasificacion)
                .Include(x => x.Funciones)
                .ThenInclude (x => x.Sala)
                .FirstOrDefaultAsync(m => m.Id == id);

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
            ViewBag.SelectGeneros = new SelectList(_context.Generos, "Id", "Descripcion");
            ViewBag.SelectClasificaciones = new SelectList(_context.Clasificaciones, "Id", "Descripcion");
            return View();
        }

        // POST: Peliculas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Genero,Sinopsis")] Pelicula pelicula)
        {

            //valida si ya existe el nombre
            ValidarNombreExistente(pelicula);

            if (ModelState.IsValid)
            {
                _context.Add(pelicula);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            ViewBag.SelectGeneros = new SelectList(_context.Generos, "Id", "Descripcion");
            ViewBag.SelectClasificaciones = new SelectList(_context.Clasificaciones, "Id", "Descripcion");
            return View(pelicula);
        }

        [Authorize(Roles = nameof(Role.Administrador))]
        // GET: Peliculas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pelicula = await _context.Peliculas.FindAsync(id);
            if (pelicula == null)
            {
                return NotFound();
            }
            
            ViewBag.SelectGeneros = new SelectList(_context.Generos, "Id", "Descripcion");
            ViewBag.SelectClasificaciones = new SelectList(_context.Clasificaciones, "Id", "Descripcion");
            return View(pelicula);
        }

        // POST: Peliculas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Sinopsis, ClasificacionId, Clasificacion")] Pelicula pelicula)
        {
            if (id != pelicula.Id)
            {
                return NotFound();
            }

            //valida si ya existe el nombre
            ValidarNombreExistente(pelicula);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pelicula);
                    await _context.SaveChangesAsync();
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

            ViewBag.SelectGeneros = new SelectList(_context.Generos, "Id", "Descripcion");
            ViewBag.SelectClasificaciones = new SelectList(_context.Clasificaciones, "Id", "Descripcion");
            return View(pelicula);
        }

        //[Authorize(Roles = nameof(Role.Administrador))]
        // GET: Peliculas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pelicula = await _context
                .Peliculas
                .Include(x => x.Genero)
                .Include(x => x.Clasificacion)
                .FirstOrDefaultAsync(m => m.Id == id);

            
            if (pelicula == null)
            {
                return NotFound();
            }

            return View(pelicula);
        }

        // POST: Peliculas/Delete/5
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
               .Include(x => x.Genero)
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
    }
}
