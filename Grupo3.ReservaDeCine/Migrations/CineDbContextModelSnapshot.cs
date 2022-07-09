﻿// <auto-generated />
using System;
using Grupo3.ReservaDeCine.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Grupo3.ReservaDeCine.Migrations
{
    [DbContext(typeof(CineDbContext))]
    partial class CineDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Grupo3.ReservaDeCine.Models.Clasificacion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasMaxLength(10);

                    b.Property<int>("EdadMinima");

                    b.HasKey("Id");

                    b.ToTable("Clasificaciones");
                });

            modelBuilder.Entity("Grupo3.ReservaDeCine.Models.Funcion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CantButacasDisponibles");

                    b.Property<DateTime>("Fecha");

                    b.Property<DateTime>("Horario");

                    b.Property<int>("PeliculaId");

                    b.Property<int>("SalaId");

                    b.HasKey("Id");

                    b.HasIndex("PeliculaId");

                    b.HasIndex("SalaId");

                    b.ToTable("Funciones");
                });

            modelBuilder.Entity("Grupo3.ReservaDeCine.Models.Genero", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.ToTable("Generos");
                });

            modelBuilder.Entity("Grupo3.ReservaDeCine.Models.Pelicula", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ClasificacionId");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("Sinopsis")
                        .HasMaxLength(2000);

                    b.HasKey("Id");

                    b.HasIndex("ClasificacionId");

                    b.ToTable("Peliculas");
                });

            modelBuilder.Entity("Grupo3.ReservaDeCine.Models.PeliculaGenero", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("GeneroId");

                    b.Property<int>("PeliculaId");

                    b.HasKey("Id");

                    b.HasIndex("GeneroId");

                    b.HasIndex("PeliculaId");

                    b.ToTable("PeliculaGeneros");
                });

            modelBuilder.Entity("Grupo3.ReservaDeCine.Models.Reserva", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CantButacas");

                    b.Property<int>("ClienteId");

                    b.Property<decimal>("CostoTotal");

                    b.Property<DateTime>("FechaDeAlta");

                    b.Property<int>("FuncionId");

                    b.HasKey("Id");

                    b.HasIndex("ClienteId");

                    b.HasIndex("FuncionId");

                    b.ToTable("Reservas");
                });

            modelBuilder.Entity("Grupo3.ReservaDeCine.Models.Sala", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CapacidadTotal");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("TipoId");

                    b.HasKey("Id");

                    b.HasIndex("TipoId");

                    b.ToTable("Salas");
                });

            modelBuilder.Entity("Grupo3.ReservaDeCine.Models.TipoSala", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<decimal>("PrecioEntrada");

                    b.HasKey("Id");

                    b.ToTable("TiposSala");
                });

            modelBuilder.Entity("Grupo3.ReservaDeCine.Models.Usuario", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Apellido")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<DateTime?>("FechaUltimoAcceso");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<byte[]>("Password");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.HasKey("Id");

                    b.ToTable("Usuarios");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Usuario");
                });

            modelBuilder.Entity("Grupo3.ReservaDeCine.Models.Administrador", b =>
                {
                    b.HasBaseType("Grupo3.ReservaDeCine.Models.Usuario");

                    b.Property<int>("Legajo");

                    b.HasDiscriminator().HasValue("Administrador");
                });

            modelBuilder.Entity("Grupo3.ReservaDeCine.Models.Cliente", b =>
                {
                    b.HasBaseType("Grupo3.ReservaDeCine.Models.Usuario");

                    b.Property<DateTime>("FechaDeAlta");

                    b.Property<DateTime>("FechaDeNacimiento");

                    b.HasDiscriminator().HasValue("Cliente");
                });

            modelBuilder.Entity("Grupo3.ReservaDeCine.Models.Funcion", b =>
                {
                    b.HasOne("Grupo3.ReservaDeCine.Models.Pelicula", "Pelicula")
                        .WithMany("Funciones")
                        .HasForeignKey("PeliculaId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Grupo3.ReservaDeCine.Models.Sala", "Sala")
                        .WithMany("Funciones")
                        .HasForeignKey("SalaId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Grupo3.ReservaDeCine.Models.Pelicula", b =>
                {
                    b.HasOne("Grupo3.ReservaDeCine.Models.Clasificacion", "Clasificacion")
                        .WithMany("Peliculas")
                        .HasForeignKey("ClasificacionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Grupo3.ReservaDeCine.Models.PeliculaGenero", b =>
                {
                    b.HasOne("Grupo3.ReservaDeCine.Models.Genero", "Genero")
                        .WithMany("Peliculas")
                        .HasForeignKey("GeneroId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Grupo3.ReservaDeCine.Models.Pelicula", "Pelicula")
                        .WithMany("Generos")
                        .HasForeignKey("PeliculaId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Grupo3.ReservaDeCine.Models.Reserva", b =>
                {
                    b.HasOne("Grupo3.ReservaDeCine.Models.Cliente", "Cliente")
                        .WithMany("Reservas")
                        .HasForeignKey("ClienteId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Grupo3.ReservaDeCine.Models.Funcion", "Funcion")
                        .WithMany("Reservas")
                        .HasForeignKey("FuncionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Grupo3.ReservaDeCine.Models.Sala", b =>
                {
                    b.HasOne("Grupo3.ReservaDeCine.Models.TipoSala", "Tipo")
                        .WithMany()
                        .HasForeignKey("TipoId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
