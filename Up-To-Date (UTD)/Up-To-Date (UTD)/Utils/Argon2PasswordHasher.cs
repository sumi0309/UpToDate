using Konscious.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Serilog;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Up_To_Date__UTD_.Utils
{
    public class Argon2PasswordHasher : IPasswordHasher<IdentityUser>
    {
        private const int SaltSize = 16;   // 16 bytes salt
        private const int KeySize = 32;    // 32 bytes (256-bit) key
        private const int Iterations = 4;  // Number of iterations (you can adjust this for security)
        private const int DegreeOfParallelism = 8; // The number of threads used by Argon2

        public string HashPassword(IdentityUser user, string password)
        {
            Log.Information("Hashing password for user: {UserName}", user.UserName);
            byte[] salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            using (var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password)))
            {
                argon2.Salt = salt;
                argon2.DegreeOfParallelism = DegreeOfParallelism;
                argon2.Iterations = Iterations;
                argon2.MemorySize = 65536;
                byte[] hash = argon2.GetBytes(KeySize);
                byte[] hashBytes = new byte[SaltSize + KeySize];
                Buffer.BlockCopy(salt, 0, hashBytes, 0, SaltSize);
                Buffer.BlockCopy(hash, 0, hashBytes, SaltSize, KeySize);
                string hashedPassword = Convert.ToBase64String(hashBytes);
                Log.Information("Password hashed successfully for user: {UserName}", user.UserName);
                return hashedPassword;
            }
        }

        public PasswordVerificationResult VerifyHashedPassword(IdentityUser user, string hashedPassword, string providedPassword)
        {
            Log.Information("Verifying hashed password for user: {UserName}", user.UserName);
            byte[] hashBytes = Convert.FromBase64String(hashedPassword);
            byte[] salt = new byte[SaltSize];
            byte[] storedHash = new byte[KeySize];
            Buffer.BlockCopy(hashBytes, 0, salt, 0, SaltSize);
            Buffer.BlockCopy(hashBytes, SaltSize, storedHash, 0, KeySize);
            using (var argon2 = new Argon2id(Encoding.UTF8.GetBytes(providedPassword)))
            {
                argon2.Salt = salt;
                argon2.DegreeOfParallelism = DegreeOfParallelism;
                argon2.Iterations = Iterations;
                argon2.MemorySize = 65536;
                byte[] hash = argon2.GetBytes(KeySize);
                for (int i = 0; i < KeySize; i++)
                {
                    if (storedHash[i] != hash[i])
                    {
                        Log.Warning("Password verification failed for user: {UserName}", user.UserName);
                        return PasswordVerificationResult.Failed;
                    }
                }
                Log.Information("Password verification successful for user: {UserName}", user.UserName);
                return PasswordVerificationResult.Success;
            }
        }
    }
}
