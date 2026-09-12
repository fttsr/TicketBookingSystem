using EventService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventService.Application.Interfaces
{
    public interface IEventRepository
    {
        Task<List<Event>> GetAllAsync();
        Task<Event?> GetByIdAsync(Guid id);
        Task AddAsync(Event eventItem);
        Task DeleteAsync(Event eventItem);
        Task SaveChangesAsync();
    }
}
