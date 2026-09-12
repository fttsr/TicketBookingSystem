using AutoMapper;
using EventService.Application.DTOs;
using EventService.Application.Interfaces;
using EventService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventService.Application.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _repository;
        private readonly IMapper _mapper;

        public EventService(IEventRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<EventDto?> CancelReservationAsync(Guid id)
        {
            var eventItem = await _repository.GetByIdAsync(id);

            if (eventItem is null)
                return null;

            eventItem.CancelReservation();

            await _repository.SaveChangesAsync();

            return _mapper.Map<EventDto>(eventItem);
        }

        public async Task<EventDto> CreateAsync(CreateAndUpdateEventDto dto)
        {
            var eventItem = _mapper.Map<Event>(dto);

            eventItem.Id = Guid.NewGuid();
            eventItem.AvailableTickets = eventItem.TotalTickets;

            await _repository.AddAsync(eventItem);
            await _repository.SaveChangesAsync();

            return _mapper.Map<EventDto>(eventItem);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var eventItem = await _repository.GetByIdAsync(id);

            if (eventItem is null)
                return false;

            await _repository.DeleteAsync(eventItem);
            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<List<EventDto>> GetAllAsync()
        {
            var events = await _repository.GetAllAsync();
            return _mapper.Map<List<EventDto>>(events);
        }

        public async Task<EventDto?> GetByIdAsync(Guid id)
        {
            var eventItem = await _repository.GetByIdAsync(id);
            return _mapper.Map<EventDto>(eventItem);
        }

        public async Task<EventDto?> ReserveTicketAsync(Guid id)
        {
            var eventItem = await _repository.GetByIdAsync(id);

            if (eventItem is null)
                return null;

            eventItem.ReserveTicket();

            await _repository.SaveChangesAsync();

            return _mapper.Map<EventDto>(eventItem);
        }

        public async Task<EventDto?> UpdateAsync(Guid id, CreateAndUpdateEventDto dto)
        {
            var eventItem = await _repository.GetByIdAsync(id);

            if (eventItem is null)
                return null;

            _mapper.Map(dto, eventItem);

            await _repository.SaveChangesAsync();

            return _mapper.Map<EventDto>(eventItem);
        }
    }
}
