using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grupo3.ReservaDeCine.Helpers
{
    public static class MensajesError
    {
        public const string Requerido = "El campo {0} es requerido";
        public const string MaxLenght = "La longitud máxima de un {0} es de {1} caracteres";
        public const string OnlyLetters = "Formato inválido. El {0} sólo admite letras";
        public const string FormatoInvalido = "Formato inválido";
        public const string OnlyNumbers = "Formato inválido. El {0} sólo admite números";
    }
}
