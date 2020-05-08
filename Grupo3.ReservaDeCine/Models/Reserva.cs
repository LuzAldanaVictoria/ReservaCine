using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Grupo3.ReservaDeCine.Models
{
    public class Reserva
    {
        [Key]
        [Display(Name = "ID")]
        public int Id { get; set; }


        [Required(ErrorMessage = "El campo Usuario es requerido")]
        [Display(Name = "Usuario")]
        public Usuario Usuario { get; set; }


        [Required(ErrorMessage = "El campo Función es requerido")]
        [Display(Name = "Función")]
        public Funcion Funcion { get; set; }


        [Required(ErrorMessage = "El campo Cantidade de Butacas es requerido")]
        [Display(Name = "Cantidad de butacas")]
        public int CantButacas { get; set; }


        [Display(Name = "Costo total")]
        public decimal CostoTotal { get; set; }


        [Display(Name = "Fecha de alta")]
        public DateTime FechaDeAlta { get; set; }


    }
    
}