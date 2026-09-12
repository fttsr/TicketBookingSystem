using EventService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventService.Application.Interfaces
{
    public interface IEventService
    {
        Task<List<EventDto>> GetAllAsync();
        Task<EventDto?> GetByIdAsync(Guid id);
        Task<EventDto> CreateAsync(CreateAndUpdateEventDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<EventDto?> UpdateAsync(Guid id, CreateAndUpdateEventDto dto);
        Task<EventDto?> ReserveTicketAsync(Guid id);
        Task<EventDto?> CancelReservationAsync(Guid id);
    }
}
