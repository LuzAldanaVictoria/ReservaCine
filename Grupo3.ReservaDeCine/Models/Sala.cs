using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Grupo3.ReservaDeCine.Models
{
    public class Sala
    {
        [Key]
        [Display(Name = "ID")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo Nombre es requerido")]
        [MaxLength(50, ErrorMessage = "La longitud máxima de un Nombre es de 50 caracteres")]
        [MinLength(2, ErrorMessage = "La longitud mínima de un Nombre es de 2 caracteres")]
        [Display(Name = "Nombre")]
        
        public string Nombre { get; set; }


        [Required(ErrorMessage = "El campo Tipo de Sala")]
        [Display(Name = "Tipo de Sala")]
        public string Tipo { get; set; }


        [Required(ErrorMessage = "El campo Capacidad Total es requerido")]
        [Display(Name = "Capacidad Total (cantidad de butacas)")]
        [Range(1,999,ErrorMessage = "La capacidad de la sala debe ser entre 1 y 999")]
        public int CapacidadTotal { get; set; }


        [Display(Name = "Funciones")] 
        public List<Funcion> Funciones { get; set; }

        
 
        public Sala ()
        {
            Funciones = new List<Funcion>();
        }
    }
}