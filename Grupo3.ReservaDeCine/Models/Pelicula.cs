using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Grupo3.ReservaDeCine.Models
{
    public class Pelicula
    {
        [Key]
        [Display(Name = "ID")]
        public int Id { get; set; }


        [Required(ErrorMessage = "El campo Nombre es requerido")]
        [MaxLength(100, ErrorMessage = "La longitud máxima de un Nombre es de 100 caracteres")]
        [MinLength(2, ErrorMessage = "La longitud mínima de un Nombre es de 2 caracteres")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }


        [Required(ErrorMessage = "El campo Género es requerido")]
        [Display(Name = "Género")]
        public Genero Genero { get; set; }


        [Display(Name = "Sinopsis")]
        [MaxLength(250, ErrorMessage = "La longitud máxima de la Sinopsis es de 250 caracteres")]
        public string Sinopsis { get; set; }



        [Display(Name = "Funciones")]
        public List<Funcion> Funciones { get; set; }
       


        public Pelicula ()
        {
            Funciones = new List<Funcion>();
        }
    }
}