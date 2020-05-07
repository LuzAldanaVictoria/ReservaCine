using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Grupo3.ReservaDeCine.Models
{
    public class Genero
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "El campo Descripción es requerido")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }


        [Display(Name = "Películas")]
        public List<Pelicula> Peliculas { get; set; }


        public Genero ()
        {
            Peliculas = new List<Pelicula>();
        }
    }
}