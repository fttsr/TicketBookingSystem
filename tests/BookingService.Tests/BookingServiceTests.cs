using AutoMapper;
using BookingService.Application.DTOs;
using BookingService.Application.Interfaces;
using BookingService.Domain.Entities;
using BookingService.Domain.Enums;
using Moq;
using Xunit.Sdk;

namespace BookingService.Tests
{
    public class BookingServiceTests
    {
        private readonly Mock<IBookingRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IEventClient> _eventClientMock;
        private readonly Mock<IEventPublisher> _eventPublisherMock;

        private readonly Application.Services.BookingService _service;

        public BookingServiceTests()
        {
            _repositoryMock = new Mock<IBookingRepository>();
            _mapperMock = new Mock<IMapper>();
            _eventClientMock = new Mock<IEventClient>();
            _eventPublisherMock = new Mock<IEventPublisher>();

            _service = new Application.Services.BookingService(
                _repositoryMock.Object,
                _mapperMock.Object,
                _eventClientMock.Object,
                _eventPublisherMock.Object);
        }


        [Fact]
        public async Task CreateAsync_WhenEventExistsAndTicketReserved_CreatesBooking()
        {
            var eventId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var email = "test@mock.com";

            var dto = new CreateAndUpdateBookingDto
            {
                EventId = eventId,
            };

            var eventDto = new EventDto
            {
                Id = eventId,
                Title = "Pantera Reunion 2026",
                Date = DateTime.UtcNow.AddDays(10),
                Venue = "Dallas, TX"
            };

            var booking = new Booking
            {
                EventId = eventId
            };

            var bookingDto = new BookingDto
            {
                EventId = eventId
            };

            _eventClientMock.Setup(client => client.GetByIdAsync(eventId)).ReturnsAsync(eventDto);
            _eventClientMock.Setup(client => client.ReserveTicketAsync(eventId)).ReturnsAsync(true);

            _mapperMock.Setup(mapper => mapper.Map<Booking>(dto)).Returns(booking);
            _mapperMock.Setup(mapper => mapper.Map<BookingDto>(booking)).Returns(bookingDto);

            var result = await _service.CreateAsync(dto, userId, email);

            Assert.Equal(eventId, result.EventId);

            _repositoryMock.Verify(repository => repository.AddAsync(booking), Times.Once);
            _repositoryMock.Verify(repository => repository.SaveChangesAsync(), Times.Once);

            _eventPublisherMock.Verify(
                publisher => publisher.PublishBookingCreatedAsync(
                    booking.Id,
                    userId,
                    email,
                    booking.EventId,
                    eventDto.Title,
                    eventDto.Date,
                    eventDto.Venue),
                 Times.Once);
        }


        [Fact]
        public async Task CreateAsync_WhenEventNotFound_ThrowsInvalidOperationException()
        {
            var eventId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var email = "test@mock.com";

            var dto = new CreateAndUpdateBookingDto
            {
                EventId = eventId
            };

            _eventClientMock.Setup(client => client.GetByIdAsync(eventId)).ReturnsAsync((EventDto?)null);

            Func<Task> action = () => _service.CreateAsync(dto, userId, email);

            await Assert.ThrowsAsync<InvalidOperationException>(action);

            _repositoryMock.Verify(repository => repository.AddAsync(It.IsAny<Booking>()), Times.Never);
            _repositoryMock.Verify(repository => repository.SaveChangesAsync(), Times.Never);

            _eventPublisherMock.Verify(
                publisher => publisher.PublishBookingCreatedAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<string>())
                , Times.Never);
        }


        [Fact]
        public async Task CreateAsync_WhenTicketReservationFails_ThrowsInvalidOperationException()
        {
            var eventId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var email = "test@mock.com";

            var dto = new CreateAndUpdateBookingDto
            {
                EventId = eventId
            };

            var eventDto = new EventDto
            {
                Id = eventId,
                Title = "Pantera Reunion 2026",
                Date = DateTime.UtcNow.AddDays(10),
                Venue = "Dallas, TX"
            };

            _eventClientMock.Setup(client => client.GetByIdAsync(eventId)).ReturnsAsync(eventDto);
            _eventClientMock.Setup(client => client.ReserveTicketAsync(eventId)).ReturnsAsync(false);

            Func<Task> action = () => _service.CreateAsync(dto, userId, email);

            await Assert.ThrowsAsync<InvalidOperationException>(action);

            _repositoryMock.Verify(repository => repository.AddAsync(It.IsAny<Booking>()), Times.Never);
            _repositoryMock.Verify(repository => repository.SaveChangesAsync(), Times.Never);

            _eventPublisherMock.Verify(
                publisher => publisher.PublishBookingCreatedAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<string>()),
                Times.Never);
        }


        [Fact]
        public async Task CancelAsync_WhenBookingExists_CancelsBooking()
        {
            var bookingId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var email = "test@mock.com";

            var booking = new Booking
            {
                Id = bookingId,
                UserId = userId,
                EventId = eventId,
                Status = BookingStatus.Confirmed
            };

            var eventDto = new EventDto
            {
                Id = eventId,
                Title = "Pantera Reunion 2026",
                Date = DateTime.UtcNow.AddDays(10),
                Venue = "Dallas, TX"
            };

            var bookingDto = new BookingDto
            {
                Id = bookingId,
                EventId = eventId
            };

            _repositoryMock
                .Setup(repository => repository.GetByIdAsync(bookingId))
                .ReturnsAsync(booking);

            _eventClientMock
                .Setup(client => client.GetByIdAsync(eventId))
                .ReturnsAsync(eventDto);

            _eventClientMock
                .Setup(client => client.CancelReservationAsync(eventId))
                .ReturnsAsync(true);

            _mapperMock
                .Setup(mapper => mapper.Map<BookingDto>(booking))
                .Returns(bookingDto);

            var result = await _service.CancelAsync(bookingId, userId, email);

            Assert.NotNull(result);
            Assert.Equal(BookingStatus.Cancelled, booking.Status);

            _repositoryMock.Verify(repository => repository.SaveChangesAsync(), Times.Once);

            _eventPublisherMock.Verify(
                publisher => publisher.PublishBookingCancelledAsync(
                    bookingId,
                    userId,
                    email,
                    booking.EventId,
                    eventDto.Title,
                    eventDto.Date,
                    eventDto.Venue),
                Times.Once);
        }

        [Fact]
        public async Task CancelAsync_WhenBookingNotFound_ReturnsNull()
        {
            var bookingId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var email = "test@mock.com";

            _repositoryMock.Setup(repository => repository.GetByIdAsync(bookingId)).ReturnsAsync((Booking?)null);

            var result = await _service.CancelAsync(bookingId, userId, email);

            Assert.Null(result);

            _eventClientMock.Verify(client => client.GetByIdAsync(It.IsAny<Guid>()), Times.Never);

            _repositoryMock.Verify(repository => repository.SaveChangesAsync(), Times.Never);

            _eventPublisherMock.Verify(
                publisher => publisher.PublishBookingCancelledAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<String>()),
                Times.Never);
        }

        [Fact]
        public async Task CancelAsync_WhenBookingNotBelongs_ReturnsNull()
        {
            var bookingId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();
            var email = "test@mock.com";

            var booking = new Booking
            {
                Id = bookingId,
                UserId = ownerId,
                EventId = Guid.NewGuid(),
                Status = BookingStatus.Confirmed
            };

            _repositoryMock.Setup(repository => repository.GetByIdAsync(bookingId)).ReturnsAsync(booking);

            var result = await _service.CancelAsync(bookingId, userId, email);

            Assert.Null(result);

            _eventClientMock.Verify(client => client.GetByIdAsync(It.IsAny<Guid>()), Times.Never);

            _repositoryMock.Verify(repository => repository.SaveChangesAsync(), Times.Never);

            _eventPublisherMock.Verify(
                publisher => publisher.PublishBookingCancelledAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<String>()),
                Times.Never);
        }
    }
}
