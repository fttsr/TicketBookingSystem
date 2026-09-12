using System;
using System.Collections.Generic;
using System.Text;

namespace TicketBookingSystem.Contracts.Events
{
    public class BookingCancelledEvent
    {
        public Guid BookingId { get; set; }
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public Guid EventId { get; set; }
        public string EventTitle { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }

        public string Venue { get; set; } = string.Empty;

    }
}
