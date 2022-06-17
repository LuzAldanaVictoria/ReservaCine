using Grupo3.ReservaDeCine.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Grupo3.ReservaDeCine.Models
{
    public class Pelicula
    {
        [Key]
        [Display(Name = "ID")]
        public int Id { get; set; }


        [Required(ErrorMessage = MensajesError.Requerido)]
        [MaxLength(Constantes.MAX_LENGTH_10, ErrorMessage = "La longitud máxima de un Nombre es de 100 caracteres")]
        [MinLength(Constantes.MIN_LENGTH_2, ErrorMessage = "La longitud mínima de un Nombre es de 2 caracteres")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }


        [Display(Name = "Géneros")]
        public List<PeliculaGenero> Generos { get; set; }


        [MaxLength(Constantes.MAX_LENGTH_250, ErrorMessage ="La longitud máxima de la sinopsis es de 250 caracteres")]
        [Display(Name = "Sinopsis")]
        public string Sinopsis { get; set; }


        [ForeignKey("Clasificacion")]
        [Required(ErrorMessage = MensajesError.Requerido)]
        [Display(Name = "Clasificación")]
        public int ClasificacionId { get; set; }
        public Clasificacion Clasificacion { get; set; }


        [Display(Name = "Funciones")]
        public List<Funcion> Funciones { get; set; }
       

    }
}