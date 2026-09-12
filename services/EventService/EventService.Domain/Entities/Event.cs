using System;
using System.Collections.Generic;
using System.Text;

namespace EventService.Domain.Entities
{
    public class Event
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Venue { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int TotalTickets { get; set; }
        public int AvailableTickets { get; set; }

        public void ReserveTicket()
        {
            if (AvailableTickets <= 0)
                throw new InvalidOperationException("Нет доступных билетов.");
            AvailableTickets--;
        }

        public void CancelReservation()
        {
            if (AvailableTickets >= TotalTickets)
                throw new InvalidOperationException("Невозможно вернуть билет: все билеты доступны.");
            AvailableTickets++;
        }
    }
}
