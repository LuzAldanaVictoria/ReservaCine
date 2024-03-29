﻿using Grupo3.ReservaDeCine.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Grupo3.ReservaDeCine.Models
{
    public class Reserva
    {
        [Key]
        [Display(Name = "ID")]
        public int Id { get; set; }



        [Required(ErrorMessage = MensajesError.Requerido)]
        [Range (1,10, ErrorMessage = "La cantidad de butacas debe ser entre 1 y 10")]
        [Display(Name = "Cantidad de butacas")]
        public int CantButacas { get; set; }


        [Display(Name = "Costo total")]
        public decimal CostoTotal { get; set; }


        [Display(Name = "Fecha de alta")]
        public DateTime FechaDeAlta { get; set; }


        [ForeignKey("Cliente")]
        [Display(Name = "Cliente")]
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }


        [ForeignKey("Funcion")]
        [Required(ErrorMessage = MensajesError.Requerido)]
        [Display(Name = "Función")]
        public int FuncionId { get; set; }
        public Funcion Funcion { get; set; }



    }

}