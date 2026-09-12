using System;
using System.Collections.Generic;
using System.Text;

namespace BookingService.Application.Interfaces
{
    public interface IEventPublisher
    {
        Task PublishBookingCreatedAsync(Guid bookingId, Guid userId, string email, Guid eventId, string eventTitle, DateTime eventDate, string Venue);
        Task PublishBookingCancelledAsync(Guid bookingId, Guid userId, string email, Guid eventId, string eventTitle, DateTime eventDate, string Venue);
    }
}
