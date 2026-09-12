using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationService.Application.DTOs
{
    public class BookingCancelledNotificationDto
    {
        public string Email { get; set; } = string.Empty;
        public Guid BookingId { get; set; }
        public string EventTitle { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Venue { get; set; } = string.Empty;
    }
}
