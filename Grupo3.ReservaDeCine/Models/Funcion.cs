using Grupo3.ReservaDeCine.Helpers;
using System;
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


        [Required(ErrorMessage = MensajesError.Requerido)]
        [Display(Name = "Fecha")]
        public DateTime Fecha { get; set; }


        [Required(ErrorMessage = MensajesError.Requerido)]
        [Display(Name = "Horario")]
        public DateTime Horario { get; set; }


        [Display(Name = "Butacas disponibles")]
        public int CantButacasDisponibles { get; set; }


        [Display(Name = "Reservas")]
        public List<Reserva> Reservas { get; set; }

        [ForeignKey("Sala")]
        [Required(ErrorMessage = MensajesError.Requerido)]
        [Display(Name = "Sala")]
        public int SalaId { get; set; }
        public Sala Sala { get; set; }


        [ForeignKey("Pelicula")]
        [Required(ErrorMessage = MensajesError.Requerido)]
        [Display(Name = "Película")]
        public int PeliculaId { get; set; }
        public Pelicula Pelicula { get; set; }


        [NotMapped]
        [Display(Name = "Fecha y Hora")]
        public string FechaHora
        {
            get
            {
                return $"{Fecha:dd/MM/yyyy} {Horario:HH:mm}";
            }
        }





    }
}