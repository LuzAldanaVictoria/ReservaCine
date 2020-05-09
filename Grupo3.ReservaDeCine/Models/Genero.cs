using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Grupo3.ReservaDeCine.Models
{
    public class Genero
    {
        [Key]
        [Display(Name = "ID")]
        public int Id { get; set; }


        [Required(ErrorMessage = "El campo Descripción es requerido")]
        [MaxLength(50, ErrorMessage = "La longitud máxima de un Apellido es de 50 caracteres")]
        [MinLength(5, ErrorMessage = "La longitud mínima de un Apellido es de 5 caracteres")]
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