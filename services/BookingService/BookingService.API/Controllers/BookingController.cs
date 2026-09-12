using BookingService.Application.DTOs;
using BookingService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookingService.API.Controllers
{

    [ApiController]
    [Route("/api/bookings")]
    [Authorize]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _service;

        public BookingController(IBookingService service)
        {
            _service = service;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim is null)
                throw new UnauthorizedAccessException();

            return Guid.Parse(userIdClaim.Value);
        }

        private string GetCurrentUserEmail()
        {
            var userEmailClaim = User.FindFirst(ClaimTypes.Email);

            if (userEmailClaim is null)
                throw new UnauthorizedAccessException();

            return userEmailClaim.Value;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAndUpdateBookingDto dto)
        {
            var userId = GetCurrentUserId();
            var email = GetCurrentUserEmail();

            var created = await _service.CreateAsync(dto, userId, email);

            return CreatedAtAction(
                nameof(GetById), 
                new { id = created.Id }, 
                created);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = GetCurrentUserId();
            return Ok(await _service.GetByUserIdAsync(userId));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userId = GetCurrentUserId();

            var booking = await _service.GetByIdAsync(id, userId);

            return booking is null
                ? NotFound()
                : Ok(booking);
        }

        [HttpPost("{id:guid}/cancel")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var userId = GetCurrentUserId();
            var email = GetCurrentUserEmail();

            var booking = await _service.CancelAsync(id, userId, email);

            return booking is null
                ? NotFound()
                : Ok(booking);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory()
        {
            var userId = GetCurrentUserId();
            return Ok(await _service.GetHistoryAsync(userId));
        }

        
    }
}
