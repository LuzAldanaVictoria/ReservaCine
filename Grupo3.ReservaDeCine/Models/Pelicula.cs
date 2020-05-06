using System;
using System.Collections.Generic;
using System.Text;

namespace Grupo3.ReservaDeCine.Models
{
    public class Pelicula
    {
        public Sala Sala { get; set; }
        public List<Funcion> Funciones { get; set; }
        public string Nombre { get; set; }
        public Genero Genero { get; set; }


        public Pelicula ()
        {
            Funciones = new List<Funcion>();
        }
    }
}