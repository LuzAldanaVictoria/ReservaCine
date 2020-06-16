using Grupo3.ReservaDeCine.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grupo3.ReservaDeCine.Database
{
    public class CineDbContext : DbContext
    {
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Administrador> Administradores { get; set; }
        public DbSet<PeliculaGenero> PeliculaGeneros { get; set; }
        public DbSet<Sala> Salas { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<Pelicula> Peliculas { get; set; }
        public DbSet<Genero> Generos { get; set; }
        public DbSet<Funcion> Funciones { get; set; }
        public DbSet<TipoSala> TiposSala { get; set; }
        public DbSet<Clasificacion> Clasificaciones { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public IEnumerable Roles { get; internal set; }

        public CineDbContext(DbContextOptions options) : base(options) { }

        public static implicit operator CineDbContext(Reserva v)
        {
            throw new NotImplementedException();
        }
    }
}
