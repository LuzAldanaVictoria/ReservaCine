using Grupo3.ReservaDeCine.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Grupo3.ReservaDeCine.Models
{
    public class Usuario
    {

        [Key]
        public int Id { get; set; }


        [Required]
        [Display(Name = "Rol")]
        public Role Role { get; set; }


        [Required]
        [MaxLength(30, ErrorMessage = "La longitud máxima de Usuario es de 30 caracteres")]
        [Display(Name = "Nombre de Usuario")]
        public string Username { get; set; }


        [ScaffoldColumn(false)]
        [Display(Name = "Fecha de último acceso")]
        public DateTime? FechaUltimoAcceso { get; set; }


        [ScaffoldColumn(false)]
        [Display (Name = "Contraseña")]
        public byte[] Password { get; set; }





    }
}
