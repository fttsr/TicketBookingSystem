using NotificationService.Application.DTOs;
using NotificationService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IEmailSender _emailSender;

        public NotificationService(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public async Task SendBookingCancelledAsync(BookingCancelledNotificationDto dto)
        {
            var subject = "Бронирование успешно отменено.";
            var message =
                "Ваше бронирование отменено.\n\n" +
                $"Мероприятие: {dto.EventTitle}\n" +
                $"Номер бронирования: {dto.BookingId}";

            await _emailSender.SendAsync(dto.Email, subject, message);
        }

        public async Task SendBookingCreatedAsync(BookingCreatedNotificationDto dto)
        {
            var subject = "Бронирование подтверждено!";

            var message =
                "Ваше бронирование успешно создано.\n\n" +
                $"Мероприятие: {dto.EventTitle}\n" +
                $"Дата: {dto.EventDate}\n" +
                $"Место {dto.Venue}\n" +
                $"Номер бронирования: {dto.BookingId}";

            await _emailSender.SendAsync(dto.Email, subject, message);
        }

        public async Task SendRegistrationSuccessAsync(RegistrationSuccessNotificationDto dto)
        {
            var subject = "Регистрация успешно завершена!";
            var message = "Вы успешно зарегистрировались в системе.";

            await _emailSender.SendAsync(dto.Email, subject, message);
        }
    }
}
