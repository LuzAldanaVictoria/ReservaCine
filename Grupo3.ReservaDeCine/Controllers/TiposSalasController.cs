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
    [Authorize(Roles = nameof(Role.Administrador))]
    public class TiposSalasController : Controller
    {
        private readonly CineDbContext _context;

        public TiposSalasController(CineDbContext context)
        {
            _context = context;
        }

        // GET: TiposSalas
        public IActionResult Index()
        {
            return View(_context.TiposSala.ToList());
        }

        // GET: TiposSalas/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoSala = _context.TiposSala
                .FirstOrDefault(m => m.Id == id);
            if (tipoSala == null)
            {
                return NotFound();
            }

            return View(tipoSala);
        }

        // GET: TiposSalas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TiposSalas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public  IActionResult Create([Bind("Id,Nombre,PrecioEntrada")] TipoSala tipoSala)
        {
            //valida si ya existe el nombre
            ValidarNombreExistente(tipoSala); 
            

            if (ModelState.IsValid)
            {
                _context.Add(tipoSala);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(tipoSala);
        }

        // GET: TiposSalas/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoSala = _context.TiposSala.Find(id);
            if (tipoSala == null)
            {
                return NotFound();
            }
            return View(tipoSala);
        }

        // POST: TiposSalas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Nombre,PrecioEntrada")] TipoSala tipoSala)
        {
            if (id != tipoSala.Id)
            {
                return NotFound();
            }

            ValidarNombreExistente(tipoSala);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipoSala);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoSalaExists(tipoSala.Id))
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
            return View(tipoSala);
        }

        // GET: TiposSalas/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoSala = _context.TiposSala
                .FirstOrDefault(m => m.Id == id);
            if (tipoSala == null)
            {
                return NotFound();
            }

            return View(tipoSala);
        }

        // POST: TiposSalas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var tipoSala = _context.TiposSala.Find(id);
            _context.TiposSala.Remove(tipoSala);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private bool TipoSalaExists(int id)
        {
            return _context.TiposSala.Any(e => e.Id == id);
        }

        private void ValidarNombreExistente(TipoSala tipoSala)
        {
            if (_context.TiposSala.Any(e => Comparar(e.Nombre, tipoSala.Nombre) && e.Id != tipoSala.Id))
            {
               ModelState.AddModelError(nameof(tipoSala.Nombre), "Ya existe un tipo de sala con ese nombre");
            }
        }


        //Función que compara que los nombres no sean iguales, ignorando espacios y case. 
        private bool Comparar(string s1, string s2)
        {
            return s1.Where(c => !char.IsWhiteSpace(c)).Select(char.ToUpperInvariant)
                .SequenceEqual(s2.Where(c => !char.IsWhiteSpace(c)).Select(char.ToUpperInvariant));
        }
    }
}
