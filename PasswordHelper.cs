using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OfficeFlow
{
    /// <summary>
    /// Provides utility methods for hashing and verifying passwords using the MD5 hashing algorithm.
    /// </summary>
    /// <remarks>This class includes methods to compute the MD5 hash of a password and to verify a plain-text
    /// password against an MD5 hashed password. Note that MD5 is considered cryptographically insecure and should not
    /// be used for sensitive data or secure password storage. It is recommended to use more secure hashing algorithms
    /// such as SHA-256 or bcrypt for password management.</remarks>
    internal class PasswordHelper
    {
        /// <summary>
        /// Computes the MD5 hash of the specified password and returns it as a lowercase hexadecimal string.
        /// </summary>
        /// <param name="password">The input string to hash. Cannot be <see langword="null"/> or empty.</param>
        /// <returns>A string representing the MD5 hash of the input password, formatted as a lowercase hexadecimal value.</returns>
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

        /// <summary>
        /// Verifies whether the provided plain-text password matches the specified MD5 hashed password.
        /// </summary>
        /// <remarks>This method compares the MD5 hash of the input password with the provided hashed
        /// password. Ensure that the hashed password is generated using the same MD5 hashing algorithm to avoid
        /// mismatches. Note that MD5 is considered cryptographically insecure and should not be used for sensitive
        /// data.</remarks>
        /// <param name="password">The plain-text password to verify.</param>
        /// <param name="hashedPassword">The MD5 hashed password to compare against.</param>
        /// <returns><see langword="true"/> if the MD5 hash of the provided password matches the specified hashed password;
        /// otherwise, <see langword="false"/>.</returns>
        public static Boolean VerifyMD5(string password, String hashedPassword)
        {
            // Hash des eingegebenen Passworts berechnen
            string comparableHash = HashMD5(password);

            if (comparableHash == hashedPassword)
            {
                // Passwort stimmt überein
                return true;
            }
            else
            {
                // Passwort stimmt nicht überein
                return false;
            }
        }
    }
}
