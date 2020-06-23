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

            var salas = await _context.Salas
                .Include(x => x.Tipo)
                .ToListAsync();

            return View(salas);

        }

        // GET: Salas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

                var sala = await _context.Salas
                .Include(x => x.Tipo)
                .Include(x => x.Funciones).ThenInclude(x => x.Pelicula)
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
            ViewBag.SelectTiposDeSala = new SelectList(_context.TiposSala, "Id", "Nombre");
            return View();
        }

        // POST: Salas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,TipoId,CapacidadTotal")] Sala sala)
        {

            ValidarNombreExistente(sala);

            if (ModelState.IsValid)
            {                
                    _context.Add(sala);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));                            
            }

            ViewBag.SelectTiposDeSala = new SelectList(_context.TiposSala, "Id", "Nombre");

            return View(sala);
        }

        // GET: Salas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var sala = await _context
                .Salas
                .FirstOrDefaultAsync(m => m.Id == id);

            if (sala == null)
            {
                return NotFound();
            }

            ViewBag.SelectTiposDeSala = new SelectList(_context.TiposSala, "Id", "Nombre");
            return View(sala);
        }

        // POST: Salas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Sala sala)
        {
            if (id != sala.Id)
            {
                return NotFound();
            }

            ValidarNombreExistente(sala);
            ValidarCapacidadSegunReservas(id, sala.CapacidadTotal);
           

            if (ModelState.IsValid)
            {
                

                try
                {
                    AjustarDisponibilidadDeButacasEnFunciones(id, sala.CapacidadTotal);
                    // para ajustar la cantidad de butacas disponibles en todas las funciones futuras de esa sala
                    
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

            
            ViewBag.SelectTiposDeSala = new SelectList(_context.TiposSala, "Id", "Nombre");
            return View(sala);
        }

        // GET: Salas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sala = await _context
                .Salas
                .Include(x => x.Tipo)
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
            await _context.TiposSala.ToListAsync();
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
        
        //Se valida que al querer modificar la capacidad de una sala, no existan reservas con mayor cantidad de butacas
        private void ValidarCapacidadSegunReservas(int salaId, int capacidadSalaModificada) 
        {
            var Funciones =
                _context.Funciones
                .Include (x => x.Reservas)
                .Where(x => x.Fecha > DateTime.Today && x.SalaId == salaId)
                .ToList();

            foreach(Funcion funcion in Funciones)
            {
                int sumaButacasReservadas = 0;
                
                foreach(Reserva reserva in funcion.Reservas)
                {
                    sumaButacasReservadas += reserva.CantButacas;
                }
                    
                if(sumaButacasReservadas > capacidadSalaModificada)
                {
                    ModelState.AddModelError(nameof(Sala.CapacidadTotal), "No se puede disminuir la cantidad de butacas por debajo de la cantidad ya reserva");
                    return;
                }
                                            
            }
                       
        }

        // Se usa al cambiar la capacidad de la sala, ajustando la disponibilidad en las funciones
       private void AjustarDisponibilidadDeButacasEnFunciones(int salaId, int capacidadTotal)

        {
            var Funciones =
                _context.Funciones
                .Include(x => x.Reservas)
                .Where(x => x.Fecha > DateTime.Today && x.SalaId == salaId)
                .ToList();

            foreach (Funcion funcion in Funciones)
            {
                int sumaButacasReservadas = 0;

                foreach (Reserva reserva in funcion.Reservas)
                {
                    sumaButacasReservadas += reserva.CantButacas;
                }

                funcion.CantButacasDisponibles = capacidadTotal - sumaButacasReservadas;
                
            }


        }



    }
}
