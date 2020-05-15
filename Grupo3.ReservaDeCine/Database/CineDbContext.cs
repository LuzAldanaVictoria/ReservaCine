﻿using Grupo3.ReservaDeCine.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grupo3.ReservaDeCine.Database
{
    public class CineDbContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Sala> Salas { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<Pelicula> Peliculas { get; set; }
        public DbSet<Genero> Generos { get; set; }
        public DbSet<Funcion> Funciones { get; set; }
        public DbSet<TipoSala> TipoSala { get; set; }

        public CineDbContext(DbContextOptions options) : base(options) { }


      
    }
}
