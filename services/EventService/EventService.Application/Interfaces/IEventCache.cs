using EventService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventService.Application.Interfaces
{
    public interface IEventCache
    {
        Task<EventDto?> GetAsync(Guid eventId);
        Task SetAsync(EventDto dto);
        Task RemoveAsync(Guid eventId);
    }
}
