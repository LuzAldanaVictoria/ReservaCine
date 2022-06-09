using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Grupo3.ReservaDeCine.Helpers;

namespace Grupo3.ReservaDeCine.Models
{
    public class Clasificacion
    {

        [Key]
        [Display(Name = "ID")]
        public int Id { get; set; }


        [Required(ErrorMessage = MensajesError.Requerido)]
        [MaxLength(10, ErrorMessage = "La longitud máxima de 10 caracteres")]
        [MinLength(2, ErrorMessage = "La longitud mínima es de 2 caracteres")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }


        [Required(ErrorMessage = MensajesError.Requerido)]
        [Range(0, 18, ErrorMessage = "La edad mínima debe ser entre 0 y 18")]
        [Display(Name = "Edad mínima del espectador")]
        public int EdadMinima { get; set; }


        [Display(Name = "Películas")]
        public List<Pelicula> Peliculas { get; set; }



    }
}
