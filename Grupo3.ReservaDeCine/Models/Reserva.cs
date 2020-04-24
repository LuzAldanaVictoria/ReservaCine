using System;
using System.Collections.Generic;
using System.Text;

namespace Grupo3.ReservaDeCine.Models
{
    public class Reserva
    {
        public Usuario Usuario { get; set; }
        public DateTime FechaDeAlta { get; set; }
        public decimal CostoTotal { get; set; }
        public Funcion Funcion { get; set; }
        public int CantButacas { get; set; }
    }
}