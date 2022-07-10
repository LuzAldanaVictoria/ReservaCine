using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Grupo3.ReservaDeCine.Extensions
{
    public static class StringExtensions
    {
        //metodo que creamos para encriptar la pass, con algoritmo sha256 y generando un hash.
        //Y me da un choclo irreversible y me devulve un array de bytes para dsp comparar
        public static byte[] Encriptar(this string data) =>
            new SHA256Managed().ComputeHash(Encoding.ASCII.GetBytes(data));
    }

}
