using BookingService.Application.Interfaces;
using BookingService.Domain.Entities;
using BookingService.Domain.Enums;
using BookingService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingService.Infrastructure.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly BookingDbContext _context;

        public BookingRepository(BookingDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Booking booking)
        {
            await _context.bookings.AddAsync(booking);
        }

        public async Task<List<Booking>> GetAllAsync()
        {
            return await _context.bookings.Where(b => b.Status != BookingStatus.Cancelled).ToListAsync();
        }

        public async Task<Booking?> GetByIdAsync(Guid id)
        {
            return await _context.bookings.FindAsync(id);
        }

        public async Task<List<Booking>> GetHistoryAsync(Guid userId)
        {
            return await _context.bookings.Where(b => b.UserId == userId).ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<Booking>> GetByUserIdAsync(Guid userId)
        {
            return await _context.bookings.Where(b => 
                                              b.UserId == userId &&
                                              b.Status != BookingStatus.Cancelled)
                                           .ToListAsync();
        } 
    }
}
