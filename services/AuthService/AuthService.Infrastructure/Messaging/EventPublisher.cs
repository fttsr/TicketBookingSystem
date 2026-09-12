using AuthService.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using TicketBookingSystem.Contracts.Events;

namespace AuthService.Infrastructure.Messaging
{
    public class EventPublisher : IEventPublisher
    {
        private readonly IConfiguration _configutation;

        public EventPublisher(IConfiguration configuration)
        {
            _configutation = configuration;
        }

        public async Task PublishUserRegisteredAsync(Guid userId, string email)
        {
            var factory = new ConnectionFactory
            {
                HostName = _configutation["RabbitMq:Host"],
                UserName = _configutation["RabbitMq:Username"],
                Password = _configutation["RabbitMq:Password"]
            };

            await using var connection = await factory.CreateConnectionAsync();

            await using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: "user-registered",
                durable: true,
                exclusive: false,
                autoDelete: false);

            var message = new UserRegisteredEvent
            {
                UserId = userId,
                Email = email,
            };

            var json = JsonSerializer.Serialize(message);

            var body = Encoding.UTF8.GetBytes(json);

            var properties = new BasicProperties
            {
                Persistent = true
            };

            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: "user-registered",
                body: body,
                mandatory: false,
                basicProperties: properties);
        }
    }
}
