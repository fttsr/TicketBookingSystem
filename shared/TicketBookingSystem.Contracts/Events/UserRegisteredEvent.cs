using System;
using System.Collections.Generic;
using System.Text;

namespace TicketBookingSystem.Contracts.Events
{
    public class UserRegisteredEvent
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}
