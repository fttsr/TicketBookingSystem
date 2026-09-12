 using BookingService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingService.Application.DTOs
{
    public class BookingDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid EventId { get; set; }
        public DateTime CreatedAt { get; set; }
        public BookingStatus Status { get; set; }
    }
}
