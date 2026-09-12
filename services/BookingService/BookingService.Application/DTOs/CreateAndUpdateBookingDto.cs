using BookingService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingService.Application.DTOs
{
    public class CreateAndUpdateBookingDto
    {
        public Guid EventId { get; set; }
    }
}
