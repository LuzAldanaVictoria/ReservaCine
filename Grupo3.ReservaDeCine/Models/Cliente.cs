using Grupo3.ReservaDeCine.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Grupo3.ReservaDeCine.Helpers;

namespace Grupo3.ReservaDeCine.Models
{
    public class Cliente : Usuario
    {

        [Required(ErrorMessage = MensajesError.Requerido)]
        [Display(Name = "Fecha de nacimiento")]
        public DateTime FechaDeNacimiento { get; set; }


        [Display(Name = "Fecha de Alta")]
        public DateTime FechaDeAlta { get; set; }


        [Display(Name = "Reservas")]
        public List<Reserva> Reservas { get; set; }


        [ScaffoldColumn(false)]
        public override Role Role => Role.Cliente;

    }
}