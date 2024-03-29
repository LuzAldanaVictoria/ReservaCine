﻿using Grupo3.ReservaDeCine.Helpers;
using Grupo3.ReservaDeCine.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Grupo3.ReservaDeCine.Models
{
    public abstract class Usuario
    {
        [Key]
        [Display(Name = "ID")]
        public int Id { get; set; }

        [Required(ErrorMessage = MensajesError.Requerido)]
        [MaxLength(Constantes.MAX_LENGTH_100, ErrorMessage = "La longitud máxima de un Nombre es de 100 caracteres")]
        [MinLength(Constantes.MIN_LENGTH_2, ErrorMessage = "La longitud mínima de un Nombre es de 2 caracteres")]
        [RegularExpression("[a-zA-ZZñÑáéíóúÁÉÍÓÚ]*", ErrorMessage = "Formato inválido. El Nombre sólo admite letras")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }


        [Required(ErrorMessage = MensajesError.Requerido)]
        [MaxLength(Constantes.MAX_LENGTH_100, ErrorMessage = "La longitud máxima de un Apellido es de 100 caracteres")]
        [MinLength(Constantes.MIN_LENGTH_2, ErrorMessage = "La longitud mínima de un Apellido es de 2 caracteres")]
        [RegularExpression("[a-zA-ZñÑáéíóúÁÉÍÓÚ]*", ErrorMessage = "Formato inválido. El Apellido sólo admite letras")]
        [Display(Name = "Apellido")]
        public string Apellido { get; set; }


        [Required(ErrorMessage = MensajesError.Requerido)]
        [EmailAddress(ErrorMessage = "Formato de Email inválido")]
        [Display(Name = "Email")]
        public string Email { get; set; }


        [Required(ErrorMessage = "El campo Nombre de Usuario es requerido")]
        [MaxLength(Constantes.MAX_LENGTH_20, ErrorMessage = "La longitud máxima de Usuario es de 20 caracteres")]
        [Display(Name = "Nombre de Usuario")]
        public string Username { get; set; }

        [ScaffoldColumn(false)]
        [Display (Name = "Contraseña")]
        public byte[] Password { get; set; }


        [Display(Name = "Fecha de último acceso")]
        public DateTime? FechaUltimoAcceso { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "Rol")]
        public abstract Role Role { get; }

    }
}
