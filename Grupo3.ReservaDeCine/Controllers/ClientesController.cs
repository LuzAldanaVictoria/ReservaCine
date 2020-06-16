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
using Grupo3.ReservaDeCine.Extensions;
using System.Text.RegularExpressions;

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
                id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
       
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
        public async Task<IActionResult> Create(string password, Cliente cliente)
        {
            ComprobarFechaDeNacimiento(cliente);
            ValidarUserNameExistente(cliente.Username);
            ValidarPassword(password);

            if (ModelState.IsValid)
            {
                cliente.Password = password.Encriptar();
                _context.Add(cliente);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
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
            [Authorize(Roles = nameof(Role.Administrador))]
            public IActionResult Edit(int id, Cliente cliente, string password)
            {
                return EditarCliente(id, cliente, password);
            }


            [Authorize(Roles = nameof(Role.Cliente))]
            [HttpGet]
            public IActionResult EditMe()
            {
                int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int id);
                var cliente = _context.Clientes.Find(id);
                if (cliente == null)
                {
                    return NotFound();
                }
                return View(cliente);
            }


            [Authorize(Roles = nameof(Role.Cliente))]
            [HttpPost]
            [ValidateAntiForgeryToken]
            public IActionResult EditMe(Cliente cliente, string password)
            {
                int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int id);
                return EditarCliente(id, cliente, password);
            }


        private IActionResult EditarCliente(int id, Cliente cliente, string password)
        {
            if (cliente.Id != id)
            {
                return NotFound();
            }

            ComprobarFechaDeNacimiento(cliente);
            ValidarEmailExistente(cliente);

            ModelState.Remove(nameof(Cliente.Username));

            if (!string.IsNullOrWhiteSpace(password))
            {
                ValidarPassword(password);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Cliente clienteDb = _context.Clientes.Find(cliente.Id);

                    clienteDb.Email = cliente.Email;
                    clienteDb.FechaDeNacimiento = cliente.FechaDeNacimiento;
                    clienteDb.Nombre = cliente.Nombre;
                    clienteDb.Apellido = cliente.Apellido;

                    if (!string.IsNullOrWhiteSpace(password))
                    {
                        clienteDb.Password = password.Encriptar();
                    }
             
                    _context.Update(clienteDb);
                    _context.SaveChanges();
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

                if (User.IsInRole(nameof(Role.Administrador)))
                {
                    return RedirectToAction(nameof(Index));
                }

                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            return View(cliente);
        }
   
    [Authorize(Roles = nameof(Role.Administrador))]
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


      
        private void ComprobarFechaDeNacimiento(Cliente cliente)
        {
            if (cliente.FechaDeNacimiento.Year < 1920 || cliente.FechaDeNacimiento.Year > (DateTime.Today.Year - 12))
            {
                ModelState.AddModelError(nameof(cliente.FechaDeNacimiento), "Año de nacimiento inválido");
            }
        }

        private void ValidarUserNameExistente(string username)
        {
            if (_context.Usuarios.Any(x => Comparar(x.Username, username)))
            {
                ModelState.AddModelError(nameof(username), "Nombre de usuario no disponible");
            }
        }


        //Función que compara que los nombres no sean iguales, ignorando espacios y case. 
        private static bool Comparar(string s1, string s2)
        {
            return s1.Where(c => !char.IsWhiteSpace(c)).Select(char.ToUpperInvariant)
                .SequenceEqual(s2.Where(c => !char.IsWhiteSpace(c)).Select(char.ToUpperInvariant));
        }


        public void ValidarPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError(nameof(Cliente.Password), "La contraseña es requerida.");
            }

            if (password.Length < 8)
            {
                ModelState.AddModelError(nameof(Cliente.Password), "La contraseña debe tener al menos 8 caracteres.");
            }

            bool contieneUnNumero = new Regex("[0-9]").Match(password).Success;
            bool contieneUnaMinuscula = new Regex("[a-z]").Match(password).Success;
            bool contieneUnaMayuscula = new Regex("[A-Z]").Match(password).Success;

            if (!contieneUnNumero || !contieneUnaMinuscula || !contieneUnaMayuscula)
            {
                ModelState.AddModelError(nameof(Cliente.Password), "La contraseña debe contener al menos un número, una minúscula y una mayúscula.");
            }
        }


        private void ValidarEmailExistente(Cliente cliente)
        {
            if (_context.Clientes.Any(e => Comparar(e.Email, cliente.Email) && e.Id != cliente.Id))
            {
                ModelState.AddModelError(nameof(cliente.Email), "Ya existe un cliente con este Email");
            }
        }
    }

}
