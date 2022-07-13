using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Grupo3.ReservaDeCine.Database;
using Grupo3.ReservaDeCine.Models;
using Grupo3.ReservaDeCine.Extensions;
using System.Text.RegularExpressions;
using System.Security.Claims;

namespace Grupo3.ReservaDeCine.Controllers
{
    public class AdministradoresController : Controller
    {
        private readonly CineDbContext _context;

        public AdministradoresController(CineDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            }
          
            var administrador = _context.Administradores.FirstOrDefault(x => x.Id == id);

            if (administrador == null)
            {
                return NotFound();
            }

            return View(administrador);
        }



    }
}
