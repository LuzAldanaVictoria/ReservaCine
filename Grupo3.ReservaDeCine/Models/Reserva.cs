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

        [ForeignKey("Usuario")]
        [Required(ErrorMessage = "El campo Usuario es requerido")]
        [Display(Name = "Usuario")]
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }


        [ForeignKey("Funcion")]
        [Required(ErrorMessage = "El campo Función es requerido")]
        [Display(Name = "Función")]
        public int FuncionId { get; set; }
        public Funcion Funcion { get; set; }


        [Required(ErrorMessage = "El campo Cantidade de Butacas es requerido")]
        [Display(Name = "Cantidad de butacas")]
        public int CantButacas { get; set; }


        [Display(Name = "Costo total")]
        public decimal CostoTotal { get; set; }


        [Display(Name = "Fecha de alta")]
        public DateTime FechaDeAlta { get; set; }


        [NotMapped]
        public string UsuarioButacas
        {
            get
            {
                return $"{Usuario.Nombre} {CantButacas}";
            }
        }
    }
    
}