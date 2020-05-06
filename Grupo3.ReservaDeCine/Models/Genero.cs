using System;
using System.Collections.Generic;
using System.Text;

namespace Grupo3.ReservaDeCine.Models
{
    public class Genero
    {
        public string Descripcion { get; set; }
        public List<Pelicula> Peliculas { get; set; }

        public Genero ()
        {
            Peliculas = new List<Pelicula>();
        }
    }
}