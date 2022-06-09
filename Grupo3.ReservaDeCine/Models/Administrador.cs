using Grupo3.ReservaDeCine.Helpers;
using Grupo3.ReservaDeCine.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Grupo3.ReservaDeCine.Models
{
    public class Administrador : Usuario
    {
        [Required(ErrorMessage = MensajesError.Requerido)]
        [Display(Name = "Legajo")]
        public int Legajo { get; set; }


        [ScaffoldColumn(false)]
        public override Role Role => Role.Administrador;
    }
}
