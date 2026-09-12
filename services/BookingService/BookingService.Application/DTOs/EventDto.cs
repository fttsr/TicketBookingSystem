using System;
using System.Collections.Generic;
using System.Text;

namespace BookingService.Application.DTOs
{
    public class EventDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Venue { get; set; } = string.Empty;
    }
}
