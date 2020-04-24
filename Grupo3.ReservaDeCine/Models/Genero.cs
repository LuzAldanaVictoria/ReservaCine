using System;
using System.Collections.Generic;
using System.Text;

namespace Grupo3.ReservaDeCine.Models
{
    public class Genero
    {
        public string Descripcion { get; set; }
        //podria ser un Enum, un Array o algo parecido?
        public List<Pelicula> Peliculas { get; set; }
    }
}