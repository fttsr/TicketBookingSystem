using BookingService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingService.Application.Interfaces
{
    public interface IBookingService
    {
        Task<List<BookingDto>> GetAllAsync();
        Task<BookingDto?> GetByIdAsync(Guid id, Guid userId);
        Task<BookingDto> CreateAsync(CreateAndUpdateBookingDto dto, Guid userId, string email);
        Task<BookingDto?> CancelAsync(Guid id, Guid userId, string email);
        Task<List<BookingDto>> GetHistoryAsync(Guid userId);
        Task<List<BookingDto>> GetByUserIdAsync(Guid userId);

    }
}
