using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Infrastructure.Security
{
    public class PasswordHasher : Application.Interfaces.IPasswordHasher
    {
        private readonly Microsoft.AspNetCore.Identity.PasswordHasher<User> _passwordHasher = new();
        public string Hash(User user, string password)
        {
            return _passwordHasher.HashPassword(user, password);
        }

        public bool Verify(User user, string password, string passwordHash)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, passwordHash, password);

            return result == PasswordVerificationResult.Success;
        }
    }
}
