using BookingService.Domain.Entities;
using BookingService.Domain.Enums;

namespace BookingService.Tests
{
    public class BookingTests
    {
        [Fact]
        public void Cancel_WhenBookingIsActive_StatusCancelled()
        {
            var booking = new Booking
            {
                Status = BookingStatus.Confirmed,
            };

            booking.Cancel();

            Assert.Equal(BookingStatus.Cancelled, booking.Status);
        }

        [Fact]
        public void Cancel_WhenBookingIsInactive_ThrowsInvalidOperationException()
        {
            var booking = new Booking
            {
                Status = BookingStatus.Cancelled
            };

            Action action = () => booking.Cancel();

            Assert.Throws<InvalidOperationException>(action);
        }
    }
}
