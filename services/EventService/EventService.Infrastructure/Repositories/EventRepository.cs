using EventService.Application.Interfaces;
using EventService.Domain.Entities;
using EventService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventService.Infrastructure.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly EventDbContext _context;

        public EventRepository(EventDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Event eventItem)
        {
            await _context.Events.AddAsync(eventItem);
        }

        public Task DeleteAsync(Event eventItem)
        {
            _context.Events.Remove(eventItem);
            return Task.CompletedTask;
        }

        public async Task<List<Event>> GetAllAsync()
        {
            return await _context.Events.ToListAsync();
        }

        public async Task<Event?> GetByIdAsync(Guid id)
        {
            return await _context.Events.FindAsync(id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
