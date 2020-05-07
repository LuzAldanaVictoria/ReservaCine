﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Grupo3.ReservaDeCine.Models
{
    public class Funcion
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "El campo Sala es requerido")]
        public Sala Sala { get; set; }


        [Required(ErrorMessage = "El campo Película es requerido")]
        [Display(Name = "Película")]
        public Pelicula Pelicula { get; set; }


        public List<Reserva> Reservas { get; set; }


        [Display(Name = "Cantidad de butacas disponibles")]
        public int CantButacasDisponibles { get; set; }



        public Funcion ()
        {
            Reservas = new List<Reserva>();
        }
    }
}