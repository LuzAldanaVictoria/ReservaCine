﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Grupo3.ReservaDeCine.Models
{
    public class Funcion
    {
        public Sala Sala { get; set; }
        public Pelicula Pelicula { get; set; }
        public List<Reserva> Reservas { get; set; }
        public int ButacasDisponibles { get; set; }

        public Funcion ()
        {
            Reservas = new List<Reserva>();
        }
    }
}