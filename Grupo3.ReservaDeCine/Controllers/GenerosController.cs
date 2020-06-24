using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Grupo3.ReservaDeCine.Database;
using Grupo3.ReservaDeCine.Models;
using Grupo3.ReservaDeCine.Models.Enums;
using Microsoft.AspNetCore.Authorization;

namespace Grupo3.ReservaDeCine.Controllers
{
    [Authorize(Roles = nameof(Role.Administrador))]
    public class GenerosController : Controller
    {
        private readonly CineDbContext _context;

        public GenerosController(CineDbContext context)
        {
            _context = context;
        }

        // GET: Generos
        public IActionResult Index()
        {
  
            return View(_context.Generos.ToList());
        }

        // GET: Generos/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genero = _context
                .Generos
                .Include(x => x.Peliculas).ThenInclude(x => x.Pelicula)
                .FirstOrDefault(m => m.Id == id);


            if (genero == null)
            {
                return NotFound();
            }

            return View(genero);
        }

        // GET: Generos/Create
        [Authorize(Roles = nameof(Role.Administrador))]
        public IActionResult Create()
        {

            return View();
        }

        // POST: Generos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Descripcion")] Genero genero)
        {

            //validacion
            ValidarNombreExistente(genero);
       
            if (ModelState.IsValid)
            {
                _context.Add(genero);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(genero);
        }

        // GET: Generos/Edit/5
        public  IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genero =  _context.Generos.Find(id);

            if (genero == null)
            {
                return NotFound();
            }
            return View(genero);
        }

        // POST: Generos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(Role.Administrador))]
        public IActionResult Edit(int id, [Bind("Id,Descripcion")] Genero genero)
        {
            
            if (id != genero.Id)
            {
                return NotFound();
            }


            ValidarNombreExistente(genero);


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(genero);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GeneroExists(genero.Id))
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
            return View(genero);
        }

        // GET: Generos/Delete/5
        [Authorize(Roles = nameof(Role.Administrador))]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var genero = _context.Generos
                .FirstOrDefault(m => m.Id == id);
            if (genero == null)
            {
                return NotFound();
            }

            return View(genero);
        }

        // POST: Generos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var genero = _context.Generos.Find(id);
            _context.Generos.Remove(genero);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private bool GeneroExists(int id)
        {
            return _context.Generos.Any(e => e.Id == id);
        }


        private void ValidarNombreExistente(Genero genero)
        {
            if (_context.Generos.Any(e => Comparar(e.Descripcion, genero.Descripcion) && e.Id != genero.Id))
            {
                ModelState.AddModelError(nameof(genero.Descripcion), "Ya existe ese género");
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
