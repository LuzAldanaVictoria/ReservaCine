using Grupo3.ReservaDeCine.Helpers;
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


        [Required(ErrorMessage = MensajesError.Requerido)]
        [MaxLength(Constantes.MAX_LENGTH_50, ErrorMessage = "La longitud máxima de 50 caracteres")]
        [MinLength(Constantes.MIN_LENGTH_4, ErrorMessage = "La longitud mínima es de 4 caracteres")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }


        [Display(Name = "Películas")]
        public List<PeliculaGenero> Peliculas { get; set; }


    }
}