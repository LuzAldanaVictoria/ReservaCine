using System;
using System.Collections.Generic;
using System.Text;

namespace Grupo3.ReservaDeCine.Models
{
    public class Sala
    {
        public string Nombre { get; set; }
        public int CapacidadTotal { get; set; }
        public List<Funcion> Funciones { get; set; }
    }
}