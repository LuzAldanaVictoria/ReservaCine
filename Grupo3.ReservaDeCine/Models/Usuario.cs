using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Grupo3.ReservaDeCine.Models
{
    public class Usuario
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "El campo Nombre es requerido")]
        [MaxLength(100, ErrorMessage = "La longitud máxima de un Nombre es de 100 caracteres")]
        [MinLength(2, ErrorMessage = "La longitud mínima de un Nombre es de 2 caracteres")]
        [RegularExpression("[a-zA-Z]*", ErrorMessage = "Formato inválido. El Nombre sólo amdite letras")]
        public string Nombre { get; set; }


        [Required(ErrorMessage = "El campo Apellido es requerido")]
        [MaxLength(100, ErrorMessage = "La longitud máxima de un Apellido es de 100 caracteres")]
        [MinLength(2, ErrorMessage = "La longitud mínima de un Apellido es de 2 caracteres")]
        [RegularExpression("[a-zA-Z]*", ErrorMessage = "Formato inválido. El Apellido sólo amdite letras")]
        public string Apellido { get; set; }


        [Required(ErrorMessage = "El campo Email es requerido")]
        [EmailAddress(ErrorMessage = "Formato de Email inválido")]
        public string Email { get; set; }


        [Required(ErrorMessage = "El campo Fecha de Nacimiento es requerido")]
        [Display(Name = "Fecha de nacimiento")]
        public DateTime FechaDeNacimiento { get; set; }


        [Display(Name = "Fecha de Alta")]
        public DateTime FechaDeAlta { get; set; }
        

        public List<Reserva> Reservas { get; set; }

      

    
        public Usuario ()
        {
            Reservas = new List<Reserva>();
        }
    }
}