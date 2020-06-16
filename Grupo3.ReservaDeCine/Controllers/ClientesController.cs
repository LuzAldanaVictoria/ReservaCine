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
using System.Security.Claims;

namespace Grupo3.ReservaDeCine.Controllers
{
   
    public class ClientesController : Controller 
    {
        private readonly CineDbContext _context;

        public ClientesController(CineDbContext context)
        {
            _context = context;
        }


        // GET: clientes
        [Authorize(Roles = nameof(Role.Administrador))]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Clientes.ToListAsync());
        }

       
       
        
        // GET: clientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {


            if (id == null)
            {
                return NotFound();
            }

            // Esta es la consulta de la base de datos que hacemos a través de Entity Framework.
            // Toda la información que obtengamos de la base será la información que podamos acceder luego desde la variable cliente.
            // Las propiedades de navegación nos servirán para poder "Navegar" desde una propiedad a otra, en este caso desde cliente a sus "Reservas".
            // Para que las propiedades de Navegación sean cargadas es necesario utilizar el Include indicando qué propiedad queremos que se cargue (en este caso Reservas).
            // Luego, si deseamos obtener información específica de algo asociado a la reserva (en nuestro caso la función y dentro de la función la película) debemos hacer include también de eso.
            var cliente = await _context.Clientes
                .Include(x => x.Reservas).ThenInclude(x => x.Funcion).ThenInclude(x => x.Pelicula)
                .FirstOrDefaultAsync(m => m.Id == id);
            // Es importante remarcar que la semántica es => Traeme los clientes => con sus reservas => de sus reservas incluíme las funciones => de esas funciones incluíme la película.

            // Luego utilizaremos esta información en la vista.

            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }


        // GET: clientes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: clientes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Apellido,Email,FechaDeNacimiento")] Cliente cliente)
        {

            ComprobarFechaDeNacimiento(cliente);
            ValidarEmailExistente(cliente);

            if (ModelState.IsValid)
            {
                cliente.FechaDeAlta = DateTime.Now;
                _context.Add(cliente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }


        // GET: clientes/Edit/5
        [Authorize(Roles = nameof(Role.Cliente))]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

        // POST: clientes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(Role.Cliente))]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Apellido,Email,FechaDeNacimiento,FechaDeAlta")] Cliente cliente)
        {
            if (id != cliente.Id)
            {
                return NotFound();
            }
         
            ComprobarFechaDeNacimiento(cliente);
            ValidarEmailExistente(cliente);
       

            if (ModelState.IsValid)
            {
           
                try
                {
                    var clienteDb = _context.Clientes.Find(id);
                    cliente.FechaDeAlta = clienteDb.FechaDeAlta;

                    _context.Update(clienteDb);
                    await _context.SaveChangesAsync();
                }

                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(cliente.Id))
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
            return View(cliente);
        }

        // GET: clientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // POST: clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClienteExists(int id)
        {
            return _context.Clientes.Any(e => e.Id == id);
        }

        private void ComprobarFechaDeNacimiento (Cliente cliente)
        {
            if (cliente.FechaDeNacimiento.Year < 1920 || cliente.FechaDeNacimiento.Year > (DateTime.Today.Year - 12))
            {
                ModelState.AddModelError(nameof(cliente.FechaDeNacimiento), "Año de nacimiento inválido");
            }
        }

        private void ValidarEmailExistente (Cliente cliente)
        {
            if (_context.Clientes.Any(e => Comparar(e.Email, cliente.Email) && e.Id != cliente.Id))
            {
                ModelState.AddModelError(nameof(cliente.Email), "Ya existe un cliente con este Email");
            }
        }

        private bool Comparar(string s1, string s2)
        {
            return s1.Where(c => !char.IsWhiteSpace(c)).Select(char.ToUpperInvariant)
                .SequenceEqual(s2.Where(c => !char.IsWhiteSpace(c)).Select(char.ToUpperInvariant));
        }



        [HttpGet]
        [Authorize(Roles = nameof(Role.Cliente))]
        public IActionResult MiPerfil()
        {
            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int id);
            var cliente = _context.Clientes.Find(id);

               //.Include(x => x.Reservas).ThenInclude(x => x.Funcion).ThenInclude(x => x.Pelicula)
               //.FirstOrDefaultAsync(m => m.Id == id);


            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }

       
    }
}
