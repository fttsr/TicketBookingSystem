using BookingService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingService.Application.Interfaces
{
    public interface IEventClient
    {
        Task<bool> ReserveTicketAsync(Guid eventId);
        Task<bool> CancelReservationAsync(Guid eventId);
        Task<EventDto?> GetByIdAsync(Guid eventId);
    }
}
