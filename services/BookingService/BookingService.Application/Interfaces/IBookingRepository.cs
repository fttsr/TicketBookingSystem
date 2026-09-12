using BookingService.Application.DTOs;
using BookingService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingService.Application.Interfaces
{
    public interface IBookingRepository
    {
        Task<List<Booking>> GetAllAsync();
        Task<Booking?> GetByIdAsync(Guid id);
        Task AddAsync(Booking booking);
        Task SaveChangesAsync();
        Task<List<Booking>> GetHistoryAsync(Guid userId);
        Task<List<Booking>> GetByUserIdAsync(Guid userId);

    }
}
