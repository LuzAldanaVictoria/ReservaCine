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


        [Display(Name = "Nombre")]
        public String Nombre { get; set; }


        [Display(Name = "Precio de entrada")]
        public decimal PrecioEntrada { get; set; }
    }
}
