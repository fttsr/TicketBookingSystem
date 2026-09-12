using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using TicketBookingSystem.Contracts.Events;
using NotificationService.Application.Interfaces;
using NotificationService.Application.DTOs;

namespace NotificationService.Infrastructure.Messaging
{
    public class UserRegisteredConsumer : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;

        public UserRegisteredConsumer(IConfiguration configuration, IServiceScopeFactory scopeFactory)
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

            var connection = await factory.CreateConnectionAsync();

            var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: "user-registered",
                durable: true,
                exclusive: false,
                autoDelete: false);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (_, args) =>
            {
                try
                {
                    var json = Encoding.UTF8.GetString(args.Body.ToArray());

                    var message = JsonSerializer.Deserialize<UserRegisteredEvent>(json);

                    if (message is null)
                        return;

                    using var scope = _scopeFactory.CreateScope();

                    var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                    await notificationService.SendRegistrationSuccessAsync(
                        new RegistrationSuccessNotificationDto
                        {
                            Email = message.Email
                        });

                    await channel.BasicAckAsync(
                        deliveryTag: args.DeliveryTag,
                        multiple: false);
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
                queue: "user-registered",
                autoAck: true,
                consumer: consumer);

            await Task.Delay(
                Timeout.Infinite,
                stoppingToken);
        }
    }
}
