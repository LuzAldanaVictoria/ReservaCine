using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Grupo3.ReservaDeCine.Models
{
    public class Cliente
    {
        [Key]
        [Display(Name = "ID")]
        public int Id { get; set; }


        [Required(ErrorMessage = "El campo Nombre es requerido")]
        [MaxLength(100, ErrorMessage = "La longitud máxima de un Nombre es de 100 caracteres")]
        [MinLength(2, ErrorMessage = "La longitud mínima de un Nombre es de 2 caracteres")]
        [RegularExpression("[a-zA-ZZñÑáéíóúÁÉÍÓÚ]*", ErrorMessage = "Formato inválido. El Nombre sólo admite letras")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }


        [Required(ErrorMessage = "El campo Apellido es requerido")]
        [MaxLength(100, ErrorMessage = "La longitud máxima de un Apellido es de 100 caracteres")]
        [MinLength(2, ErrorMessage = "La longitud mínima de un Apellido es de 2 caracteres")]
        [RegularExpression("[a-zA-ZñÑáéíóúÁÉÍÓÚ]*", ErrorMessage = "Formato inválido. El Apellido sólo admite letras")]
        [Display(Name = "Apellido")]
        public string Apellido { get; set; }


        [Required(ErrorMessage = "El campo Email es requerido")]
        [EmailAddress(ErrorMessage = "Formato de Email inválido")]
        [Display(Name = "Email")]
        public string Email { get; set; }


        [Required(ErrorMessage = "El campo Fecha de Nacimiento es requerido")]
        [Display(Name = "Fecha de nacimiento")]
        public DateTime FechaDeNacimiento { get; set; }


        [Display(Name = "Fecha de Alta")]
        public DateTime FechaDeAlta { get; set; }


        [Display(Name = "Reservas")]
        public List<Reserva> Reservas { get; set; }


    }
}