using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Service.Helpers
{
    /// <summary>
    /// Provides cryptographic utilities used for password storage and integrity verification.
    /// </summary>
    public static class CryptographyHelper
    {
        /// <summary>
        /// Computes a SHA-256 hash of the given password and returns it as a lowercase hex string.
        /// </summary>
        /// <param name="password">The plain-text password to hash.</param>
        /// <returns>
        /// A 64-character lowercase hex string (SHA-256 digest), or <see cref="string.Empty"/>
        /// if <paramref name="password"/> is <c>null</c> or empty.
        /// </returns>
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return string.Empty;

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
