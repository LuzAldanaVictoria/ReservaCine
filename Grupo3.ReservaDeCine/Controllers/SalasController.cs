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
    public class SalasController : Controller
    {
        private readonly CineDbContext _context;

        public SalasController(CineDbContext context)
        {
            _context = context;
        }

        // GET: Salas
        public async Task<IActionResult> Index()
        {

            await _context.TiposSala.ToListAsync();

            return View(await _context.Salas.ToListAsync());

        }

        // GET: Salas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sala = await _context.Salas
                .FirstOrDefaultAsync(m => m.Id == id);
          
            if (sala == null)
            {
                return NotFound();
            }

            return View(sala);
        }

        // GET: Salas/Create
        public IActionResult Create()
        {
            ViewBag.TiposDeSala = new SelectList(_context.TiposSala, "Id", "Nombre");
            return View();
        }

        // POST: Salas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,TipoId,CapacidadTotal")] Sala sala)
        {
            //valida si ya existe el nombre
            //if (SalaNombreExists(sala.Nombre, sala.Id))
            //{
            //    ModelState.AddModelError(nameof(sala.Nombre), "Ya existe una sala con ese nombre");
            //}

            ValidarNombreExistente(sala);


            if (ModelState.IsValid)
            {                
                    _context.Add(sala);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));                            
            }

            ViewBag.TiposDeSala = new SelectList(_context.TiposSala, "Id", "Nombre");

            return View(sala);
        }

        // GET: Salas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sala = await _context.Salas.FindAsync(id);
            if (sala == null)
            {
                return NotFound();
            }
            ViewBag.TiposDeSala = new SelectList(_context.TiposSala, "Id", "Nombre");
            return View(sala);
        }

        // POST: Salas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,TipoSala,CapacidadTotal")] Sala sala)
        {
            if (id != sala.Id)
            {
                return NotFound();
            }


            ValidarNombreExistente(sala);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sala);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalaExists(sala.Id))
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

            ViewBag.TiposDeSala = new SelectList(_context.TiposSala, "Id", "Nombre");
            return View(sala);
        }

        // GET: Salas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sala = await _context.Salas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sala == null)
            {
                return NotFound();
            }

            return View(sala);
        }

        // POST: Salas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sala = await _context.Salas.FindAsync(id);
            _context.Salas.Remove(sala);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SalaExists(int id)  
        {
            return _context.Salas.Any(e => e.Id == id);
        }

        private bool SalaNombreExists(String nombreSala, int id)  
        {
            // se valida que no exista una sala con el mismo nombre, ignorando mayusculas y minisculas
            return _context.Salas.Any(e => e.Nombre.Equals(nombreSala, StringComparison.CurrentCultureIgnoreCase) &&  e.Id != id);  // De esta forma estoy recorriendo como si fuera un for la lista de salas. e.Nombre me trae el nombre del elemento en una posicion
        }



        //Función que compara que los nombres no sean iguales, ignorando espacios y case. 
        private static bool Comparar(string s1, string s2)
        {
            return s1.Where(c => !char.IsWhiteSpace(c)).Select(char.ToUpperInvariant)
                .SequenceEqual(s2.Where(c => !char.IsWhiteSpace(c)).Select(char.ToUpperInvariant));
        }


        private void ValidarNombreExistente(Sala sala)
        {
            if (_context.Salas.Any(e => Comparar(e.Nombre, sala.Nombre) && e.Id != sala.Id))
            {
                ModelState.AddModelError(nameof(sala.Nombre), "Ya existe una sala con ese nombre");
            }
        }



    }
}
