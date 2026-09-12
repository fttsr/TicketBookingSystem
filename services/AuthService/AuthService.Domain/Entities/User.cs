using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = String.Empty;
        public string PasswordHash { get; set; } = String.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
