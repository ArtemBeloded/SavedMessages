﻿using SavedMessages.Application.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SavedMessages.Persistence.Services
{
    public sealed class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 10000;

        private static readonly HashAlgorithmName Alrorithm = HashAlgorithmName.SHA512;

        public string Hash(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Alrorithm, HashSize);

            return $"{Convert.ToHexString(hash)}-{Convert.ToHexString(salt)}";
        }

        public bool Verify(string password, string passwordHash)
        {
            string[] parts = passwordHash.Split('-');
            byte[] hash = Convert.FromHexString(parts[0]);
            byte[] salt = Convert.FromHexString(parts[1]);

            byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Alrorithm, HashSize);

            return CryptographicOperations.FixedTimeEquals(hash, inputHash);
        }
    }
}