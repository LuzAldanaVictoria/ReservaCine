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

        [HttpGet]
        [Authorize(Roles = nameof(Role.Administrador))] // nameOf para no hardcodear y obtener en nombre del enum
        public IActionResult Index()
        {
            var peliculas = _context
               .Peliculas
               .Include(x => x.Generos)
               .Include(x => x.Clasificacion)
               .ToList();

            return View(peliculas);
        }


        [HttpGet]
        [Authorize(Roles = nameof(Role.Administrador))]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pelicula = _context.Peliculas
                .Include(x => x.Generos).ThenInclude(x => x.Genero)
                .Include(x => x.Clasificacion)
                .Include(x => x.Funciones).ThenInclude(x => x.Sala)
                .FirstOrDefault(x => x.Id == id);

            if (pelicula == null)
            {
                return NotFound();
            }

            return View(pelicula);
        }
    

        [HttpGet]
        [Authorize(Roles = nameof(Role.Administrador))]
        public IActionResult Create()
        { // para mostrarme el formulario, necesita la info de generos y clasificaciones. 
            ViewBag.SelectGeneros = new MultiSelectList(_context.Generos, nameof(Genero.Id), nameof(Genero.Descripcion));
            ViewBag.SelectClasificaciones = new SelectList(_context.Clasificaciones, "Id", "Descripcion");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(Role.Administrador))]
        public IActionResult Create(Pelicula pelicula, List<int> generoIds)
        {

            ValidarNombreExistente(pelicula);
            ValidarGeneros(generoIds); //valida que se seleccione al menos uno

            if (ModelState.IsValid)
            {
                pelicula.Generos = new List<PeliculaGenero>(); // creo lista de generos

                foreach (var generoId in generoIds)  //para c/u de los generos que vienen, lo agrego enla lista
                {
                    pelicula.Generos.Add(new PeliculaGenero { Pelicula = pelicula, GeneroId = generoId });
                }

                _context.Add(pelicula);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            //Si el modelo no es valido, se los guarda para volver a mostrarlos 
            ViewBag.SelectGeneros = new MultiSelectList(_context.Generos, nameof(Genero.Id), nameof(Genero.Descripcion), generoIds);
            ViewBag.SelectClasificaciones = new SelectList(_context.Clasificaciones, "Id", "Descripcion", pelicula.ClasificacionId);
            return View(pelicula);
        }


        [HttpGet]
        [Authorize(Roles = nameof(Role.Administrador))]
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(Role.Administrador))]
        public IActionResult Edit(int id, Pelicula pelicula, List<int> generoIds)
        {
            if (id != pelicula.Id)
            {
                return NotFound();
            }

            ValidarNombreExistente(pelicula);
            ValidarGeneros(generoIds);

            if (ModelState.IsValid)
            {
                try
                { //tengo que buscar la que esta en la base para modificarla
                    var peliculaDb = _context
                        .Peliculas
                        .Include(x => x.Generos)
                        .FirstOrDefault(x => x.Id == id);

                    //cambio los valores viejos por los nuevos
                    peliculaDb.Nombre = pelicula.Nombre;
                    peliculaDb.ClasificacionId = pelicula.ClasificacionId;
                    peliculaDb.Sinopsis = pelicula.Sinopsis;
                   
                    
                    foreach (var peliculaGenero in peliculaDb.Generos)
                    {//saca par ala pelcula, las peliculas-genero que habia antes en la bd
                        _context.Remove(peliculaGenero);
                    }

                    foreach (var generoId in generoIds)
                    {//agrego los nuevos generos a pelicula-genero
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
            // genero un tipo de dato SelectList,le paso las clasificaciones, y como valor toma la prop ID y mostrale al usuario la descripcion
            ViewBag.SelectClasificaciones = new SelectList(_context.Clasificaciones, "Id", "Descripcion");

            return View(pelicula);
        }


        [HttpGet]
        [Authorize(Roles = nameof(Role.Administrador))]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pelicula = _context
                .Peliculas
                .Include(x => x.Generos).ThenInclude(x => x.Genero)
                .Include(x => x.Clasificacion)
                .Include(x => x.Funciones).ThenInclude(x => x.Sala)
                .FirstOrDefault(x => x.Id == id);
            
            if (pelicula == null)
            {
                return NotFound();
            }

            return View(pelicula);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(Role.Administrador))]
        public IActionResult DeleteConfirmed(int id)
        {
            var pelicula = _context.Peliculas.Find(id);
            _context.Peliculas.Remove(pelicula);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [AllowAnonymous]
        public  IActionResult Cartelera()
        {
            var cartelera =  _context
               .Peliculas
               .Include(x => x.Generos)
               .Include(x => x.Clasificacion)
               .Include(x => x.Funciones)
               .ToList();

            return View(cartelera);
        }



        ////---- Métodos privados para validaciones ----////

        private bool PeliculaExists(int id)
        {
            return _context.Peliculas.Any(x => x.Id == id);
        }


        private void ValidarNombreExistente(Pelicula pelicula)
        {
            if (_context.Peliculas.Any(x => Comparar(x.Nombre, pelicula.Nombre) && x.Id != pelicula.Id))
            {
                ModelState.AddModelError(nameof(pelicula.Nombre), "Ya existe una película con ese nombre");
            }
        }


        private static bool Comparar(string s1, string s2)
        {
            return s1.Where(c => !char.IsWhiteSpace(c)).Select(char.ToUpperInvariant)
                .SequenceEqual(s2.Where(c => !char.IsWhiteSpace(c)).Select(char.ToUpperInvariant));
        }


        //Validar que se seleccione al menos un genero
        private void ValidarGeneros(List<int> generoIds)
        {
            if (generoIds.Count == 0)
            {
                ModelState.AddModelError(nameof(Pelicula.Generos), "La pelicula debe tener al menos un género.");
            }
        }
    }
}
