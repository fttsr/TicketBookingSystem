using BookingService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingService.Domain.Entities
{
    public class Booking
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid EventId { get; set; }
        public DateTime CreatedAt { get; set; }
        public BookingStatus Status { get; set; }

        public void Cancel()
        {
            if (Status == BookingStatus.Cancelled)
                throw new InvalidOperationException("Бронирование уже отменено.");

            Status = BookingStatus.Cancelled;
        }
    }
}
