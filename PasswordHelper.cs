using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OfficeFlow
{
    internal class PasswordHelper
    {
        public static string HashMD5(string password)
        {
            // MD5-Hashing initialisieren
            MD5 md5 = MD5.Create();

            // Hash berechnen
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashBytes = md5.ComputeHash(passwordBytes);
            string hashedPassword = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

            // Hash zurückgeben
            return hashedPassword;
        }

        public static Boolean VerifyMD5(string password, String hashedPassword)
        {
            string comparableHash = HashMD5(password);
            if (comparableHash == hashedPassword)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
