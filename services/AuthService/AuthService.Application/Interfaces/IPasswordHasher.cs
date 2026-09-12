using AuthService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Application.Interfaces
{
    public interface IPasswordHasher
    {
        string Hash(User user, string password);

        bool Verify(User user, string password, string passwordHash);
    }
}
