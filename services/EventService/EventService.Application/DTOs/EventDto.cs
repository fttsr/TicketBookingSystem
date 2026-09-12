using System;
using System.Collections.Generic;
using System.Text;

namespace EventService.Application.DTOs
{
    public class EventDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Venue { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int AvailableTickets { get; set; }
    }
}
