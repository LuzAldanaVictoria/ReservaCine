using Grupo3.ReservaDeCine.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Grupo3.ReservaDeCine.Models
{
    public class TipoSala
    {
        [Key]
        [Display(Name = "ID")]
        public int Id { get; set; }


        [Required(ErrorMessage = MensajesError.Requerido)]
        [MaxLength(Constantes.MAX_LENGTH_20, ErrorMessage = "La longitud máxima de 20 caracteres")]
        [MinLength(Constantes.MIN_LENGTH_2, ErrorMessage = "La longitud mínima es de 2 caracteres")]
        [Display(Name = "Nombre")]
        public String Nombre { get; set; }


        [Required(ErrorMessage = MensajesError.Requerido)]
        [Range(0, (double)decimal.MaxValue, ErrorMessage = "El precio de la entrada no puede ser negativo")]
        [Display(Name = "Precio de entrada")]
        public decimal PrecioEntrada { get; set; }
    }
}
