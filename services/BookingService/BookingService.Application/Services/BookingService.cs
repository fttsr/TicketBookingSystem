using AutoMapper;
using BookingService.Application.DTOs;
using BookingService.Application.Interfaces;
using BookingService.Domain.Entities;
using BookingService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingService.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _repository;
        private readonly IMapper _mapper;
        private readonly IEventClient _eventClient;
        private readonly IEventPublisher _eventPublisher;

        public BookingService(IBookingRepository repository, 
            IMapper mapper, 
            IEventClient client,
            IEventPublisher eventPublisher)
        {
            _repository = repository;
            _mapper = mapper;
            _eventClient = client;
            _eventPublisher = eventPublisher;
        }

        public async Task<BookingDto?> CancelAsync(Guid id, Guid userId, string email)
        {
            var booking = await _repository.GetByIdAsync(id);

            if (booking is null || booking.UserId != userId)
                return null;

            var eventInfo = await _eventClient.GetByIdAsync(booking.EventId);

            if (eventInfo is null)
                throw new InvalidOperationException("Мероприятие не найдено.");

            booking.Cancel();

            var cancelledInEventService = await _eventClient.CancelReservationAsync(booking.EventId);

            if (!cancelledInEventService)
                throw new InvalidOperationException("Не удалось вернуть билет в EventService.");

            await _repository.SaveChangesAsync();

            await _eventPublisher.PublishBookingCancelledAsync(
                booking.Id, 
                userId, 
                email, 
                booking.EventId,
                eventInfo.Title,
                eventInfo.Date,
                eventInfo.Venue
                );

            return _mapper.Map<BookingDto>(booking);
        }

        public async Task<BookingDto> CreateAsync(CreateAndUpdateBookingDto dto, Guid userId, string email)
        {
            var eventInfo = await _eventClient.GetByIdAsync(dto.EventId);

            if (eventInfo is null)
                throw new InvalidOperationException("Мероприятие не найдено.");

            var reserved = await _eventClient.ReserveTicketAsync(dto.EventId);

            if (!reserved)
            {
                throw new InvalidOperationException("Не удалось зарезервировать билет.");
            }

            var booking = _mapper.Map<Booking>(dto);

            booking.Id = Guid.NewGuid();
            booking.UserId = userId;
            booking.CreatedAt = DateTime.UtcNow;
            booking.Status = BookingStatus.Confirmed;

            await _repository.AddAsync(booking);
            await _repository.SaveChangesAsync();

            await _eventPublisher.PublishBookingCreatedAsync(
                booking.Id, 
                userId, email, 
                booking.EventId, 
                eventInfo.Title, 
                eventInfo.Date, 
                eventInfo.Venue);

            return _mapper.Map<BookingDto>(booking);
        }

        public async Task<List<BookingDto>> GetAllAsync()
        {
            var bookings = await _repository.GetAllAsync();

            return _mapper.Map<List<BookingDto>>(bookings);
        }

        public async Task<BookingDto?> GetByIdAsync(Guid id, Guid userId)
        {
            var booking = await _repository.GetByIdAsync(id);

            if (booking is null || booking.UserId != userId)
                return null;

            return _mapper.Map<BookingDto>(booking);
        }

        public async Task<List<BookingDto>> GetHistoryAsync(Guid userId)
        {
            var bookings = await _repository.GetHistoryAsync(userId);

            return _mapper.Map<List<BookingDto>>(bookings);
        }

        public async Task<List<BookingDto>> GetByUserIdAsync(Guid userId)
        {
            var bookings = await _repository.GetByUserIdAsync(userId);

            return _mapper.Map<List<BookingDto>>(bookings);
        }
    }
}
