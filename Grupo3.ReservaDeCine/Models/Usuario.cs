using System;
using System.Collections.Generic;
using System.Text;

namespace Grupo3.ReservaDeCine.Models
{
    public class Usuario
    {
        public List<Reserva> Reservas { get; set; }
        public DateTime FechaDeAlta { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public DateTime FechaDeNacimiento { get; set; }

    }
}