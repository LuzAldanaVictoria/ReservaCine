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
        public static byte[] Encriptar(this string data) =>
            new SHA256Managed().ComputeHash(Encoding.ASCII.GetBytes(data));
    }

}
