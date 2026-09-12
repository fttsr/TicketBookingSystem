using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NotificationService.Application.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using TicketBookingSystem.Contracts.Events;

namespace NotificationService.Infrastructure.Messaging
{
    public class BookingCancelledConsumer : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;

        public BookingCancelledConsumer(IConfiguration configuration, IServiceScopeFactory scopeFactory)
        {
            _configuration = configuration;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
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
                queue: "booking-cancelled",
                durable: true,
                exclusive: false,
                autoDelete: false);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (_, args) =>
            {
                try
                {
                    var json = Encoding.UTF8.GetString(args.Body.ToArray());

                    var message = JsonSerializer.Deserialize<BookingCancelledEvent>(json);

                    if (message is null)
                        return;

                    using var scope = _scopeFactory.CreateScope();

                    var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                    await notificationService.SendBookingCancelledAsync(new Application.DTOs.BookingCancelledNotificationDto
                    {
                        Email = message.Email,
                        BookingId = message.BookingId,
                        EventTitle = message.EventTitle,
                        EventDate = message.EventDate,
                        Venue = message.Venue
                    });

                    await channel.BasicAckAsync(
                        deliveryTag: args.DeliveryTag,
                        multiple: false
                        );
                }
                catch (Exception)
                {
                    await channel.BasicNackAsync(
                        deliveryTag: args.DeliveryTag,
                        multiple: false,
                        requeue: true);
                }
            };

            await channel.BasicConsumeAsync(
                queue: "booking-cancelled",
                autoAck: false,
                consumer: consumer);

            await Task.Delay(
                Timeout.Infinite,
                stoppingToken);
        }
    }
}
