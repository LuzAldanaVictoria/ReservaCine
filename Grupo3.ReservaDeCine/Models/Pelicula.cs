using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Grupo3.ReservaDeCine.Models
{
    public class Pelicula
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "El campo Nombre es requerido")]
        [MaxLength(100, ErrorMessage = "La longitud máxima de un Nombre es de 100 caracteres")]
        [MinLength(2, ErrorMessage = "La longitud mínima de un Nombre es de 2 caracteres")]
        [RegularExpression("[a-zA-Z]*", ErrorMessage = "Formato inválido. El Nombre sólo amdite letras")] 
        public string Nombre { get; set; }


        [Required(ErrorMessage = "El campo Género es requerido")]
        [Display(Name = "Género")]
        public Genero Genero { get; set; }


        public string Sinopsis { get; set; }


        [Required(ErrorMessage = "El campo Sala es requerido")]
        public Sala Sala { get; set; }


        public List<Funcion> Funciones { get; set; }
       

        public Pelicula ()
        {
            Funciones = new List<Funcion>();
        }
    }
}