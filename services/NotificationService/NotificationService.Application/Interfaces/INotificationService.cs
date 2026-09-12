using NotificationService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Application.Interfaces
{
    public interface INotificationService
    {
        Task SendRegistrationSuccessAsync(RegistrationSuccessNotificationDto dto);
        Task SendBookingCreatedAsync(BookingCreatedNotificationDto dto);
        Task SendBookingCancelledAsync(BookingCancelledNotificationDto dto);
    }
}
