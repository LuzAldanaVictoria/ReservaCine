using Grupo3.ReservaDeCine.Database;
using Grupo3.ReservaDeCine.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Grupo3.ReservaDeCine.Extensions;




namespace Grupo3.ReservaDeCine.Controllers
{

    public class PreCargaDbController : Controller
    {

        private readonly CineDbContext _context;

        public PreCargaDbController(CineDbContext cineDbContext)
        {
            _context = cineDbContext;
        }


        public IActionResult Inicializar()
        {
          
            try
            {
                
                _context.Database.EnsureDeleted(); // borra la base de datos
                _context.Database.Migrate(); // crea la database y aplica las migraciones que falten aplicar
                crearSalas().Wait();
                crearGeneros().Wait();
                crearClasificacion().Wait();
                crearClientes().Wait();
                crearAdministrador().Wait();
                crearPeliculas().Wait();
                crearFunciones().Wait();
                crearReservas().Wait();

                _context.SaveChanges();
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
            TempData["datosPreCargados"] = true; // una vez que se cargaron, esto es para el cartel
            return RedirectToAction("Index", "Home");
        }

       

        private async Task crearSalas()
        {

            Sala sala1 = new Sala()
            {
                Nombre = "Sala 1",
                Tipo = new TipoSala()
                {
                    Nombre = "Premium",
                    PrecioEntrada = 550
                },
                CapacidadTotal = 20
            };
            
        
            Sala sala2 = new Sala()
            {
                Nombre = "Sala 2",
                Tipo = new TipoSala()
                {
                    Nombre = "2D",
                    PrecioEntrada = 250
                },
                CapacidadTotal = 120
            };

            Sala sala3 = new Sala()
            {
                Nombre = "Sala 3",
                Tipo = new TipoSala()
                {
                    Nombre = "3D",
                    PrecioEntrada = 350
                },
                CapacidadTotal = 100
            };

            Sala sala4 = new Sala()
            {
                Nombre = "Sala 4",
                Tipo = new TipoSala()
                {
                    Nombre = "4D",
                    PrecioEntrada = 450
                },
                CapacidadTotal = 80
            };
            _context.AddRange(new[] { sala1, sala2, sala3, sala4});

                 await _context.SaveChangesAsync();
        }

        private async Task crearGeneros()
        {

            Genero genero1 = new Genero()
            {
                Descripcion = "Infantil"
            };

            Genero genero2 = new Genero()
            {
                Descripcion = "Fantasía"
            };

            Genero genero3 = new Genero()
            {
                Descripcion = "Ciencia Ficción"
            };

            Genero genero4 = new Genero()
            {
                Descripcion = "Terror"
            };

            _context.AddRange(new[] { genero1, genero2, genero3, genero4 });
             await _context.SaveChangesAsync();

        }


        private async Task crearClasificacion()
        {
            Clasificacion atp = new Clasificacion
            {
                Descripcion = "ATP",
                EdadMinima = 0
            };


            Clasificacion mayores16 = new Clasificacion
            {
                Descripcion = "+16",
                EdadMinima = 16
            };

            Clasificacion mayores14 = new Clasificacion
            {
                Descripcion = "+14",
                EdadMinima = 14
            };

            Clasificacion mayores13 = new Clasificacion
            {
                Descripcion = "+13",
                EdadMinima = 13
            };


            _context.AddRange(new[] { atp, mayores16, mayores14, mayores13 });
            await _context.SaveChangesAsync();
        }


        private async Task crearClientes()
        {

            var Cliente1 = new Cliente()
            {
                Username = "cliente1",
                Password = "Password1!".Encriptar(),
                Nombre = "Luciano",
                Apellido = "García",
                Email = "lucianogarcia@gmail.com",
                FechaDeNacimiento = new DateTime(1992, 06, 20),
                Reservas = new List<Reserva>(),
                FechaDeAlta = DateTime.Now
            };

            var Cliente2 = new Cliente()
            {
                Username = "cliente2",
                Password = "Password1!".Encriptar(),
                Nombre = "Carlos",
                Apellido = "Pereyra",
                Email = "cp2020@gmail.com",
                FechaDeNacimiento = new DateTime(2000, 11, 04),
                Reservas = new List<Reserva>(),
                FechaDeAlta = DateTime.Now
            };

            var Cliente3 = new Cliente()
            {
                Username = "cliente3",
                Password = "Password1!".Encriptar(),
                Nombre = "Carla",
                Apellido = "Rodriguez",
                Email = "carla@gmail.com",
                FechaDeNacimiento = new DateTime(1987, 01, 03),
                Reservas = new List<Reserva>(),
                FechaDeAlta = DateTime.Now
            };


            var Cliente4 = new Cliente()
            {
                Username = "cliente4",
                Password = "Password1!".Encriptar(),
                Nombre = "Laura",
                Apellido = "Gomez",
                Email = "laurita@gmail.com",
                FechaDeNacimiento = new DateTime(2005, 05, 09),
                Reservas = new List<Reserva>(),
                FechaDeAlta = DateTime.Now
            };
            _context.AddRange(new[] { Cliente1, Cliente2, Cliente3, Cliente4 });
            await _context.SaveChangesAsync();
        }

        private async Task crearAdministrador()
        {

            var administrador1 = new Administrador()
            {
                Username = "administrador1",
                Password = "Password1!".Encriptar(),
                Nombre = "Pedro",
                Apellido = "Gonzalez",
                Email = "jcgonzalez@gmail.com",
                Legajo = 0001
            };

            _context.Add(administrador1);
            await _context.SaveChangesAsync();
        }


        private async Task crearPeliculas()
        {

            var listClasificaciones = _context.Clasificaciones.ToList();

            Pelicula pelicula1 = new Pelicula()
            {

                Nombre = "La Bella y la Bestia ",
                Generos = new List<PeliculaGenero>(),
                Sinopsis = "Bella es una brillante y guapa joven que utiliza la lectura como válvula de escape de su rutinaria vida. Cuando su padre es apresado en un misterioso castillo, " +
                                    "Bella acude en su búsqueda y se presta a ocupar su lugar. El castillo es propiedad de una atormentada Bestia que, como Bella comprobará con el tiempo, resulta ser un joven príncipe " +
                                    "bajo los efectos de un hechizo. Sólo cuando conozca el amor, el príncipe podrá volver a su verdadero cuerpo.",
                Clasificacion  = listClasificaciones[0]

             };

            Pelicula pelicula2 = new Pelicula()
            {
                Nombre = "Matrix",
                Generos = new List<PeliculaGenero>(),
                Sinopsis = "Neo es un joven pirata informático que lleva una doble vida: durante el día ejerce en una empresa de servicios informáticos, mientras que por la noche se dedica a piratear " +
                                "bases de datos y saltarse sistemas de alta seguridad. Su vida cambiará cuando una noche conozca a Trinity, una misteriosa joven que parece ser una leyenda en el mundo de los 'hackers' informáticos," +
                                " que lo llevará a Neo ante su líder: Morfeo. Así descubrirá una terrible realidad y el joven deberá decidir si unirse a la resistencia o vivir su vida como hasta ahora.",
                Clasificacion = listClasificaciones[1]
            };

            Pelicula pelicula3 = new Pelicula()
            {
                Nombre = "It",
                Generos = new List<PeliculaGenero>(),
                Sinopsis = "En el pueblo de Derry, Maine, un joven de 14 años llamado Bill Denbrough (Jaeden Martell) ayuda a su hermano pequeño, George Denbrough (Jackson Robert Scott) a hacer un barco de papel." +
                                " Bill le pide que baje al sótano por parafina para impermeabilizarlo, George baja y consigue la parafina para el barco aunque nota allí una presencia que lo asusta. Bill, con su hermano abrazándole," +
                                " unta el barco con la parafina y se lo entrega a Georgie para que vaya a jugar en la lluvia excusándose de no poder acompañarlo ya que está muy enfermo.",
                Clasificacion = listClasificaciones[2]

            };

            Pelicula pelicula4 = new Pelicula()
            {
                Nombre = "Annabelle 3",
                Generos = new List<PeliculaGenero>(),
                Sinopsis = "En 1968, los demonólogos Ed y Lorraine Warren se llevan a su casa a la muñeca poseída Annabelle" +
                " después de que dos enfermeras (Debbie y Camilla) aseguraran que la muñeca a menudo realizaba actividades " +
                "violentas en su apartamento. Durante el trayecto, la muñeca convoca a los espíritus de un cementerio" +
                " situado junto a la carretera para que ataquen a Ed, pero consigue sobrevivir. Una vez en la casa, " +
                "Annabelle es colocada en vitrina en la sala de artefactos de la pareja y bendecida por el padre Gordon" +
                "para asegurarse de que su mal está contenido",

                Clasificacion = listClasificaciones[3]
            };

            Pelicula pelicula5 = new Pelicula()
            {
                Nombre = "Lightyear",
                Generos = new List<PeliculaGenero>(),
                Sinopsis = "Buzz Lightyear se embarca en una aventura" +
                " intergaláctica con un grupo de reclutas ambiciosos y su compañero robot.",

                Clasificacion = listClasificaciones[0]
            };

            Pelicula pelicula6 = new Pelicula()
            {
                Nombre = "Volver al futuro",
                Generos = new List<PeliculaGenero>(),
                Sinopsis = "El adolescente Marty McFly es amigo de Doc, un científico que ha construido una máquina del tiempo. " +
                "Cuando los dos prueban el artefacto, un error fortuito hace que Marty llegue a 1955, año en el que sus padres iban al instituto y todavía no se habían conocido. Después de impedir su primer encuentro," +
                " Marty deberá conseguir que se conozcan y se enamoren, de lo contrario su existencia no sería posible.",

                Clasificacion = listClasificaciones[2]
            };

            Pelicula pelicula7 = new Pelicula()
            {
                Nombre = "Granizo",
                Generos = new List<PeliculaGenero>(),
                Sinopsis = "Un famoso meteorólogo de la televisión se convierte en el enemigo público número " +
                "uno cuando no logra evitar una terrible tormenta de granizo",

                Clasificacion = listClasificaciones[0]
            };

            Pelicula pelicula8 = new Pelicula()
            {
                Nombre = "Harry Potter 1",
                Generos = new List<PeliculaGenero>(),
                Sinopsis = "El día de su cumpleaños, Harry Potter descubre que es hijo de dos conocidos hechiceros" +
                ", de los que ha heredado poderes mágicos. Debe asistir a una famosa escuela de magia y hechicería, " +
                "donde entabla una amistad con dos jóvenes que se convertirán en sus compañeros de aventura. " +
                "Durante su primer año en Hogwarts, descubre que un malévolo y poderoso mago llamado Voldemort" +
                " está en busca de una piedra filosofal que alarga la vida de quien la posee.",

                Clasificacion = listClasificaciones[2]
            };

            Pelicula pelicula9 = new Pelicula()
            {
                Nombre = "Minions - Nace un villano",
                Generos = new List<PeliculaGenero>(),
                Sinopsis = "En los años 70, Gru crece siendo un gran admirador de Los salvajes seis" +
                ", un supergrupo de villanos. Para demostrarles que puede ser malvado, Gru idea un plan" +
                " con la esperanza de formar parte de la banda. Por suerte, cuenta con la ayuda de sus fieles seguidores," +
                " los Minions, siempre dispuestos a sembrar el caos.",

                Clasificacion = listClasificaciones[0]
            };

            Pelicula pelicula10 = new Pelicula()
            {
                Nombre = "Top Gun",
                Generos = new List<PeliculaGenero>(),
                Sinopsis = "Agosto de 1981. El septuagenario Forrest Tucker entra en un banco de Dallas y " +
                "hace lo que mejor sabe hacer: atracarlo con toda educación e irse tranquilamente. " +
                "El problema es que está vez quizá ha dejado un cabo suelto: dentro de la sucursal se encontraba " +
                "el policía John Hunt.",

                Clasificacion = listClasificaciones[2]
            };
            var listGeneros = _context.Generos.ToList();
         
            pelicula1.Generos.Add(new PeliculaGenero { Pelicula = pelicula1, Genero = listGeneros[0] });
            pelicula2.Generos.Add(new PeliculaGenero { Pelicula = pelicula2, Genero = listGeneros[2] });
            pelicula3.Generos.Add(new PeliculaGenero { Pelicula = pelicula3, Genero = listGeneros[3] });
            pelicula4.Generos.Add(new PeliculaGenero { Pelicula = pelicula4, Genero = listGeneros[0] });
            pelicula5.Generos.Add(new PeliculaGenero { Pelicula = pelicula5, Genero = listGeneros[0] });
            pelicula6.Generos.Add(new PeliculaGenero { Pelicula = pelicula6, Genero = listGeneros[1] });
            pelicula7.Generos.Add(new PeliculaGenero { Pelicula = pelicula7, Genero = listGeneros[2] });
            pelicula8.Generos.Add(new PeliculaGenero { Pelicula = pelicula8, Genero = listGeneros[3] });
            pelicula9.Generos.Add(new PeliculaGenero { Pelicula = pelicula9, Genero = listGeneros[0] });
            pelicula10.Generos.Add(new PeliculaGenero { Pelicula = pelicula10, Genero = listGeneros[0] });
            pelicula1.Generos.Add(new PeliculaGenero { Pelicula = pelicula1, Genero = listGeneros[1] });
            pelicula2.Generos.Add(new PeliculaGenero { Pelicula = pelicula2, Genero = listGeneros[3] });
            pelicula3.Generos.Add(new PeliculaGenero { Pelicula = pelicula3, Genero = listGeneros[2] });
            pelicula4.Generos.Add(new PeliculaGenero { Pelicula = pelicula4, Genero = listGeneros[1] });
            pelicula5.Generos.Add(new PeliculaGenero { Pelicula = pelicula5, Genero = listGeneros[2] });

            _context.AddRange(new[] { pelicula1, pelicula2, pelicula3, pelicula4, pelicula5, pelicula6, 
                pelicula7, pelicula8,pelicula9,pelicula10 });
             await _context.SaveChangesAsync();
        }


        private async Task crearFunciones()
        {
            var listSalas = _context.Salas.ToList();
            var listPeliculas = _context.Peliculas.ToList();

            var funcion1 = new Funcion()
            {
                Sala = listSalas[0],
                Pelicula = listPeliculas[0],
                Fecha = new DateTime(2022, 08, 15),
                Horario = new DateTime().AddHours(14).AddMinutes(00),
                CantButacasDisponibles = listSalas[0].CapacidadTotal - 4
            };

            var funcion2 = new Funcion()
            {
                Sala = listSalas[1],
                Pelicula = listPeliculas[0],
                Fecha = new DateTime(2022, 09, 17),
                Horario = new DateTime().AddHours(20).AddMinutes(20),
                CantButacasDisponibles = listSalas[1].CapacidadTotal - 4
            };

            var funcion3 = new Funcion()
            {


                Sala = listSalas[2],
                Pelicula = listPeliculas[0],
                Fecha = new DateTime(2022, 07, 18),
                Horario = new DateTime().AddHours(20).AddMinutes(20),
                CantButacasDisponibles = listSalas[2].CapacidadTotal - 8
            };

            var funcion4 = new Funcion()
            {
                Sala = listSalas[3],
                Pelicula = listPeliculas[0],
                Fecha = new DateTime(2022, 08, 13),
                Horario = new DateTime().AddHours(21).AddMinutes(40),
                CantButacasDisponibles = listSalas[3].CapacidadTotal - 18
            };

            var funcion5 = new Funcion()
            {
                Sala = listSalas[0],
                Pelicula = listPeliculas[1],
                Fecha = new DateTime(2022, 07, 22),
                Horario = new DateTime().AddHours(23).AddMinutes(30),
                CantButacasDisponibles = listSalas[0].CapacidadTotal
            };

            var funcion6 = new Funcion()
            {
                Sala = listSalas[1],
                Pelicula = listPeliculas[1],
                Fecha = new DateTime(2022, 07, 18),
                Horario = new DateTime().AddHours(20).AddMinutes(30),
                CantButacasDisponibles = listSalas[1].CapacidadTotal
            };
           

            var funcion7 = new Funcion()
            {
                Sala = listSalas[1],
                Pelicula = listPeliculas[1],
                Fecha = new DateTime(2022, 07, 18),
                Horario = new DateTime().AddHours(21).AddMinutes(30),
                CantButacasDisponibles = listSalas[1].CapacidadTotal
            };

            var funcion8 = new Funcion()
            {
                Sala = listSalas[0],
                Pelicula = listPeliculas[1],
                Fecha = new DateTime(2022, 10, 18),
                Horario = new DateTime().AddHours(20).AddMinutes(30),
                CantButacasDisponibles = listSalas[0].CapacidadTotal
            };

            var funcion9 = new Funcion()
            {
                Sala = listSalas[3],
                Pelicula = listPeliculas[2],
                Fecha = new DateTime(2022, 07, 18),
                Horario = new DateTime().AddHours(21).AddMinutes(30),
                CantButacasDisponibles = listSalas[3].CapacidadTotal
            };

            var funcion10 = new Funcion()
            {
                Sala = listSalas[2],
                Pelicula = listPeliculas[2],
                Fecha = new DateTime(2022, 07, 19),
                Horario = new DateTime().AddHours(21).AddMinutes(30),
                CantButacasDisponibles = listSalas[2].CapacidadTotal
            };

            var funcion11 = new Funcion()
            {
                Sala = listSalas[2],
                Pelicula = listPeliculas[3],
                Fecha = new DateTime(2022, 07, 18),
                Horario = new DateTime().AddHours(21).AddMinutes(30),
                CantButacasDisponibles = listSalas[2].CapacidadTotal
            };

            var funcion12 = new Funcion()
            {
                Sala = listSalas[1],
                Pelicula = listPeliculas[3],
                Fecha = new DateTime(2022, 07, 22),
                Horario = new DateTime().AddHours(21).AddMinutes(30),
                CantButacasDisponibles = listSalas[1].CapacidadTotal
            };

            var funcion13 = new Funcion()
            {
                Sala = listSalas[2],
                Pelicula = listPeliculas[4],
                Fecha = new DateTime(2022, 07, 05),
                Horario = new DateTime().AddHours(18).AddMinutes(30),
                CantButacasDisponibles = listSalas[2].CapacidadTotal
            };

            var funcion14 = new Funcion()
            {
                Sala = listSalas[2],
                Pelicula = listPeliculas[4],
                Fecha = new DateTime(2022, 07, 22),
                Horario = new DateTime().AddHours(18).AddMinutes(30),
                CantButacasDisponibles = listSalas[2].CapacidadTotal
            };

            var funcion15 = new Funcion()
            {
                Sala = listSalas[0],
                Pelicula = listPeliculas[5],
                Fecha = new DateTime(2022, 07, 25),
                Horario = new DateTime().AddHours(18).AddMinutes(30),
                CantButacasDisponibles = listSalas[0].CapacidadTotal
            };

            var funcion16 = new Funcion()
            {
                Sala = listSalas[1],
                Pelicula = listPeliculas[5],
                Fecha = new DateTime(2022, 07, 25),
                Horario = new DateTime().AddHours(18).AddMinutes(30),
                CantButacasDisponibles = listSalas[1].CapacidadTotal
            };

            var funcion17 = new Funcion()
            {
                Sala = listSalas[2],
                Pelicula = listPeliculas[6],
                Fecha = new DateTime(2022, 07, 25),
                Horario = new DateTime().AddHours(19).AddMinutes(30),
                CantButacasDisponibles = listSalas[2].CapacidadTotal
            };

            var funcion18 = new Funcion()
            {
                Sala = listSalas[0],
                Pelicula = listPeliculas[7],
                Fecha = new DateTime(2022, 07, 26),
                Horario = new DateTime().AddHours(18).AddMinutes(50),
                CantButacasDisponibles = listSalas[0].CapacidadTotal
            };

            var funcion19 = new Funcion()
            {
                Sala = listSalas[1],
                Pelicula = listPeliculas[7],
                Fecha = new DateTime(2022, 07, 26),
                Horario = new DateTime().AddHours(23).AddMinutes(50),
                CantButacasDisponibles = listSalas[1].CapacidadTotal
            };

            var funcion20 = new Funcion()
            {
                Sala = listSalas[1],
                Pelicula = listPeliculas[8],
                Fecha = new DateTime(2022, 07, 30),
                Horario = new DateTime().AddHours(18).AddMinutes(50),
                CantButacasDisponibles = listSalas[1].CapacidadTotal
            };

            var funcion21 = new Funcion()
            {
                Sala = listSalas[3],
                Pelicula = listPeliculas[9],
                Fecha = new DateTime(2022, 07, 30),
                Horario = new DateTime().AddHours(18).AddMinutes(50),
                CantButacasDisponibles = listSalas[3].CapacidadTotal
            };

            _context.AddRange(new[] { funcion1, funcion2, funcion3, funcion4, funcion5, funcion6,funcion7, funcion8, funcion9,
                funcion10, funcion11,funcion12,funcion13,funcion14,funcion15,funcion16,funcion17,funcion18,funcion19,funcion20,funcion21});
            await _context.SaveChangesAsync();
        }


        private async Task crearReservas()
        {

            var listClientes = _context.Clientes.ToList();
            var listFunciones = _context.Funciones.ToList();

            var reserva1 = new Reserva()
            {
                Cliente = listClientes[0],
                Funcion = listFunciones[0],
                CantButacas = 2,
                CostoTotal = 1100,
                FechaDeAlta = DateTime.Now
            };

            var reserva2 = new Reserva()
            {
                Cliente = listClientes[0],
                Funcion = listFunciones[1],
                CantButacas = 4,
                CostoTotal = 1800,
                FechaDeAlta = DateTime.Now
            };

            var reserva3 = new Reserva()
            {
                Cliente = listClientes[0],
                Funcion = listFunciones[2],
                CantButacas = 4,
                CostoTotal = 1800,
                FechaDeAlta = DateTime.Now
            };

            var reserva4 = new Reserva()
            {
                Cliente = listClientes[1],
                Funcion = listFunciones[2],
                CantButacas = 4,
                CostoTotal = 1800,
                FechaDeAlta = DateTime.Now
            };

            var reserva5 = new Reserva()
            {
                Cliente = listClientes[2],
                Funcion = listFunciones[0],
                CantButacas = 2,
                CostoTotal = 1100,
                FechaDeAlta = DateTime.Now
            };

            var reserva6 = new Reserva()
            {
                Cliente = listClientes[1],
                Funcion = listFunciones[3],
                CantButacas = 10,
                CostoTotal = 4500,
                FechaDeAlta = DateTime.Now
            };

            var reserva7 = new Reserva()
            {
                Cliente = listClientes[2],
                Funcion = listFunciones[3],
                CantButacas = 8,
                CostoTotal = 3600,
                FechaDeAlta = DateTime.Now
            };
            _context.AddRange(new[] { reserva1, reserva2, reserva3, reserva4, reserva5, reserva6, reserva7 });
             await _context.SaveChangesAsync();
        }



    }
}
