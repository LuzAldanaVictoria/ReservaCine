using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Grupo3.ReservaDeCine.Database;
using Grupo3.ReservaDeCine.Models;
using Microsoft.AspNetCore.Authorization;
using Grupo3.ReservaDeCine.Models.Enums;

namespace Grupo3.ReservaDeCine.Controllers
{
    [Authorize(Roles = nameof(Role.Administrador))]
    public class ClasificacionesController : Controller
    {
        private readonly CineDbContext _context;

        public ClasificacionesController(CineDbContext context)
        {
            _context = context;
        }

        // GET: Clasificaciones
        public  IActionResult Index()
        {

            return View(_context.Clasificaciones.ToList());
        }

        // GET: Clasificaciones/Details/5
        public  IActionResult Details(int? id)
        {
            
            if (id == null)
            {
                return NotFound();
            }

            var clasificacion = _context.Clasificaciones
                .Include(x => x.Peliculas)
                .FirstOrDefault(m => m.Id == id);

            if (clasificacion == null)
            {
                return NotFound();
            }

            return View(clasificacion);
        }

        // GET: Clasificaciones/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clasificaciones/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public  IActionResult Create([Bind("Id,Descripcion,EdadMinima")] Clasificacion clasificacion)
        {
            //validacion
            ValidarDescripcionExistente(clasificacion); 
            
            if (ModelState.IsValid)
            {
                _context.Add(clasificacion);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(clasificacion);
        }

        // GET: Clasificaciones/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clasificacion =  _context.Clasificaciones.Find(id);

            if (clasificacion == null)
            {
                return NotFound();
            }
            return View(clasificacion);
        }

        // POST: Clasificaciones/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public  IActionResult Edit(int id, [Bind("Id,Descripcion,EdadMinima")] Clasificacion clasificacion)
        {
            if (id != clasificacion.Id)
            {
                return NotFound();
            }

            ValidarDescripcionExistente(clasificacion);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(clasificacion);
                     _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClasificacionExists(clasificacion.Id))
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
            return View(clasificacion);
        }

        // GET: Clasificaciones/Delete/5
        public  IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clasificacion =  _context.Clasificaciones
                .FirstOrDefault(m => m.Id == id);
            if (clasificacion == null)
            {
                return NotFound();
            }

            return View(clasificacion);
        }

        // POST: Clasificaciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public  IActionResult DeleteConfirmed(int id)
        {
            var clasificacion =  _context.Clasificaciones.Find(id);
            _context.Clasificaciones.Remove(clasificacion);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private bool ClasificacionExists(int id)
        {
            return _context.Clasificaciones.Any(e => e.Id == id);
        }

        private void ValidarDescripcionExistente(Clasificacion clasificacion)
        {
            if (_context.Clasificaciones.Any(e => Comparar(e.Descripcion, clasificacion.Descripcion) && e.Id != clasificacion.Id))
            {
                ModelState.AddModelError(nameof(clasificacion.Descripcion), "Ya existe esta clasificación");
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
