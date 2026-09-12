using BookingService.Application.Interfaces;
using BookingService.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using TicketBookingSystem.Contracts.Events;

namespace BookingService.Infrastructure.Messaging
{
    public class EventPublisher : IEventPublisher
    {
        private readonly IConfiguration _configuration;

        public EventPublisher(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private async Task PublishAsync<T>(
            string queueName,
            T message)
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMq:Host"],
                UserName = _configuration["RabbitMq:Username"],
                Password = _configuration["RabbitMq:Password"]
            };

            await using var connection = await factory.CreateConnectionAsync();

            await using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false);

            var json = JsonSerializer.Serialize(message);

            var body = Encoding.UTF8.GetBytes(json);

            var properties = new BasicProperties
            {
                Persistent = true
            };

            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: queueName,
                body: body,
                mandatory: false,
                basicProperties: properties);
        }

        public async Task PublishBookingCancelledAsync(
            Guid bookingId,
            Guid userId,
            string email,
            Guid eventId,
            string eventTitle,
            DateTime eventDate,
            string venue)
        {
            var message = new BookingCancelledEvent
            {
                BookingId = bookingId,
                UserId = userId,
                Email = email,
                EventId = eventId,
                EventTitle = eventTitle,
                EventDate = eventDate,
                Venue = venue
            };

            await PublishAsync("booking-cancelled", message);
        }

        public async Task PublishBookingCreatedAsync(
            Guid bookingId, 
            Guid userId, 
            string email, 
            Guid eventId, 
            string eventTitle, 
            DateTime eventDate, 
            string venue)
        {
            var message = new BookingCreatedEvent
            {
                BookingId = bookingId,
                UserId = userId,
                Email = email,
                EventId = eventId,
                EventTitle = eventTitle,
                EventDate = eventDate,
                Venue = venue
            };

            await PublishAsync("booking-created", message);
        }
    }
}
