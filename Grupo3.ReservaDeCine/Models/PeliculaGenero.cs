using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Grupo3.ReservaDeCine.Models
{
    public class PeliculaGenero
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Pelicula")]
        public int PeliculaId { get; set; }
        public Pelicula Pelicula { get; set; }

        [ForeignKey("Genero")]
        public int GeneroId { get; set; }
        public Genero Genero { get; set; }


    }
}
