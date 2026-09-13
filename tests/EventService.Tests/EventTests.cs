using EventService.Domain.Entities;

namespace EventService.Tests
{
    public class EventTests
    {
        [Fact]
        public void ReserveTicket_WhenTicketIsAvailable_DecreasesAvailableTickets()
        {
            var eventItem = new Event
            {
                TotalTickets = 10,
                AvailableTickets = 10,
            };

            eventItem.ReserveTicket();

            Assert.Equal(9, eventItem.AvailableTickets);
        }

        [Fact]
        public void ReserveTicket_WhenNoTicketsAvailable_ThrowsInvalidOperationException()
        {
            var eventItem = new Event
            {
                TotalTickets = 10,
                AvailableTickets = 0,
            };

            Action action = () => eventItem.ReserveTicket();

            Assert.Throws<InvalidOperationException>(action);
        }

        [Fact]
        public void CancelReservation_WhenReservationExists_IncreasesAvailableTickets()
        {
            var eventItem = new Event
            {
                TotalTickets = 10,
                AvailableTickets = 9
            };

            eventItem.CancelReservation();

            Assert.Equal(10, eventItem.AvailableTickets);
        }

        [Fact]
        public void CancelReservation_WhenAllTicketsAvailable_ThrowsInvalidOperationException()
        {
            var eventItem = new Event
            {
                TotalTickets = 10,
                AvailableTickets = 10,
            };

            Action action = () => eventItem.CancelReservation();

            Assert.Throws<InvalidOperationException>(action);
        }
    }
}
