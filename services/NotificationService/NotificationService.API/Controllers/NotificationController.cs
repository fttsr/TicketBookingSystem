using Microsoft.AspNetCore.Mvc;
using NotificationService.Application.DTOs;
using NotificationService.Application.Interfaces;

namespace NotificationService.API.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost("registration-success")]
        public async Task<IActionResult> RegistrationSuccess(RegistrationSuccessNotificationDto dto)
        {
            await _notificationService.SendRegistrationSuccessAsync(dto);

            return Ok();
        }

        [HttpPost("booking-created")]
        public async Task<IActionResult> BookingCreated(BookingCreatedNotificationDto dto)
        {
            await _notificationService.SendBookingCreatedAsync(dto);

            return Ok();
        }

        [HttpPost("booking-cancelled")]
        public async Task<IActionResult> BookingCancelled(BookingCancelledNotificationDto dto)
        {
            await _notificationService.SendBookingCancelledAsync(dto);

            return Ok();
        }
    }
}
