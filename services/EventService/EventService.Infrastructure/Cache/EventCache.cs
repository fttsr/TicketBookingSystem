using EventService.Application.DTOs;
using EventService.Application.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace EventService.Infrastructure.Cache
{
    public class EventCache : IEventCache
    {
        private readonly IDatabase _db;

        public EventCache(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public async Task<EventDto?> GetAsync(Guid eventId)
        {
            var value = await _db.StringGetAsync($"event:{eventId}");

            if (value.IsNullOrEmpty)
                return null;

            return JsonSerializer.Deserialize<EventDto>(value.ToString());
        }

        public async Task RemoveAsync(Guid eventId)
        {
            await _db.KeyDeleteAsync($"event:{eventId}");
        }

        public async Task SetAsync(EventDto dto)
        {
            var json = JsonSerializer.Serialize(dto);

            await _db.StringSetAsync(
                $"event:{dto.Id}",
                json,
                TimeSpan.FromMinutes(10));
        }
    }
}
