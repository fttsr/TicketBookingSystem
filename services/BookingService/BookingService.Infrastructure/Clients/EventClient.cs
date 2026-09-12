using BookingService.Application.DTOs;
using BookingService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace BookingService.Infrastructure.Clients
{
    public class EventClient : IEventClient
    {
        private readonly HttpClient _httpClient;

        public EventClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> CancelReservationAsync(Guid eventId)
        {
            var response = await _httpClient.PostAsync(
                $"/api/events/{eventId}/cancel",
                null);

            return response.IsSuccessStatusCode;
        }

        public async Task<EventDto?> GetByIdAsync(Guid eventId)
        {
            return await _httpClient.GetFromJsonAsync<EventDto>(
                $"/api/events/{eventId}");
        }

        public async Task<bool> ReserveTicketAsync(Guid eventId)
        {
            var response = await _httpClient.PostAsync(
                $"/api/events/{eventId}/reserve",
                null);

            return response.IsSuccessStatusCode;
        }
    }
}
