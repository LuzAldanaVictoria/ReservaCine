﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Grupo3.ReservaDeCine.Models
{
    public class Funcion
    {
        [Key]
        [Display(Name = "ID")]
        public int Id { get; set; }

       
        [ForeignKey("Sala")]
        [Required(ErrorMessage = "El campo Sala es requerido")]
        [Display(Name = "Sala")]
        public int SalaId { get; set; }
        public Sala Sala { get; set; }


        [ForeignKey("Pelicula")]
        [Required(ErrorMessage = "El campo Película es requerido")]
        [Display(Name = "Película")]
        public int PeliculaId { get; set; }
        public Pelicula Pelicula { get; set; }


        [Required(ErrorMessage = "El campo Fecha es requerido")]
        [Display(Name = "Fecha")]
        public DateTime Fecha { get; set; }


        [Required(ErrorMessage = "El campo Fecha y Hora es requerido")]
        [Display(Name = "Horario")]
        public DateTime Horario { get; set; }


        [Display(Name = "Reservas")]
        public List<Reserva> Reservas { get; set; }


        [Display(Name = "Cantidad de butacas disponibles")]
        public int CantButacasDisponibles { get; set; }

        [Display(Name = "Fecha y Hora")]
        public String FechaHora
        {
            get
            {
                return $"{Fecha.ToString("dd/MM/yyyy")} {Horario.ToString("HH:mm")}";
            }
        }





    }
}