using System;
using System.Security.Cryptography;

namespace DBAccessLibrary
{
    public static class PasswordHash
    {
        private const int Iterations = 2_000;

        private static readonly HashAlgorithmName HashAlgo = HashAlgorithmName.SHA256;

        public static string CreateHash(string password)
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            byte[] key;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgo))
            {
                key = pbkdf2.GetBytes(32);
            }

            byte[] hashBytes = new byte[48];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(key, 0, hashBytes, 16, 32);

            return Convert.ToBase64String(hashBytes);
        }

        public static bool VerifyHash(string password, string hashedPassword)
        {
            byte[] hashBytes = Convert.FromBase64String(hashedPassword);

            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            byte[] key;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgo))
            {
                key = pbkdf2.GetBytes(32);
            }

            for (int i = 0; i < 32; i++)
            {
                if (hashBytes[i + 16] != key[i])
                    return false;
            }

            return true;
        }
    }
}
