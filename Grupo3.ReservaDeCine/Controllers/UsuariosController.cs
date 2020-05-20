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
    public class UsuariosController : Controller
    {
        private readonly CineDbContext _context;

        public UsuariosController(CineDbContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            return View(await _context.Usuarios.ToListAsync());
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Esta es la consulta de la base de datos que hacemos a través de Entity Framework.
            // Toda la información que obtengamos de la base será la información que podamos acceder luego desde la variable usuario.
            // Las propiedades de navegación nos servirán para poder "Navegar" desde una propiedad a otra, en este caso desde Usuario a sus "Reservas".
            // Para que las propiedades de Navegación sean cargadas es necesario utilizar el Include indicando qué propiedad queremos que se cargue (en este caso Reservas).
            // Luego, si deseamos obtener información específica de algo asociado a la reserva (en nuestro caso la función y dentro de la función la película) debemos hacer include también de eso.
            var usuario = await _context.Usuarios
                .Include(x => x.Reservas).ThenInclude(x => x.Funcion).ThenInclude(x => x.Pelicula)
                .FirstOrDefaultAsync(m => m.Id == id);
            // Es importante remarcar que la semántica es => Traeme los usuarios => con sus reservas => de sus reservas incluíme las funciones => de esas funciones incluíme la película.

            // Luego utilizaremos esta información en la vista.

            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Apellido,Email,FechaDeNacimiento")] Usuario usuario)
        {

            ComprobarFechaDeNacimiento(usuario);
            ValidarEmailExistente(usuario);


            if (ModelState.IsValid)
            {
                usuario.FechaDeAlta = DateTime.Now;
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Apellido,Email,FechaDeNacimiento,FechaDeAlta")] Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return NotFound();
            }

            ComprobarFechaDeNacimiento(usuario);
            ValidarEmailExistente(usuario);


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.Id))
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
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }

        private void ComprobarFechaDeNacimiento (Usuario usuario)
        {
            if (usuario.FechaDeNacimiento.Year < 1920 || usuario.FechaDeNacimiento.Year > (DateTime.Today.Year - 12))
            {
                ModelState.AddModelError(nameof(usuario.FechaDeNacimiento), "Año de nacimiento inválido");
            }
        }

        private void ValidarEmailExistente (Usuario usuario)
        {
            if (_context.Usuarios.Any(e => Comparar(e.Email, usuario.Email) && e.Id != usuario.Id))
            {
                ModelState.AddModelError(nameof(usuario.Email), "Ya existe un usuario con este Email");
            }
        }

        private bool Comparar(string s1, string s2)
        {
            return s1.Where(c => !char.IsWhiteSpace(c)).Select(char.ToUpperInvariant)
                .SequenceEqual(s2.Where(c => !char.IsWhiteSpace(c)).Select(char.ToUpperInvariant));
        }
    }
}
