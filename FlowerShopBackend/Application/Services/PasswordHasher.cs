using FlowerShop.Application.Interfaces;
using Isopoh.Cryptography.Argon2;

namespace FlowerShop.Application.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            return Argon2.Hash(password);
        }

        public bool Verify(string password, string passwordHash)
        {
            return Argon2.Verify(passwordHash, password);
        }
    }
}
