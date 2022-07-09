using Grupo3.ReservaDeCine.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Grupo3.ReservaDeCine.Models
{
    public class Sala
    {
        [Key]
        [Display(Name = "ID")]
        public int Id { get; set; }


        [Required(ErrorMessage = MensajesError.Requerido)]
        [MaxLength(Constantes.MAX_LENGTH_50, ErrorMessage = "La longitud máxima de un Nombre es de 50 caracteres")]
        [MinLength(Constantes.MIN_LENGTH_2, ErrorMessage = "La longitud mínima de un Nombre es de 2 caracteres")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }


        [Required(ErrorMessage = MensajesError.Requerido)]
        [Display(Name = "Capacidad Total")]
        [Range(15, 200, ErrorMessage = "La capacidad de la sala debe ser entre 15 y 200")]
        public int CapacidadTotal { get; set; }


        [Display(Name = "Funciones")] 
        public List<Funcion> Funciones { get; set; }


        [ForeignKey("Tipo")]
        [Display(Name = "Tipo de Sala")]
        public int TipoId { get; set; }
        public TipoSala Tipo { get; set; }



    }
}