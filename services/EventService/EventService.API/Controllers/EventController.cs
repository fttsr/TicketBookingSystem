using EventService.Application.DTOs;
using EventService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EventService.API.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventController : ControllerBase
    {
        private readonly IEventService _service;

        public EventController(IEventService service)
        {
            _service = service;
        }


        // POST action
        [HttpPost]
        public async Task<IActionResult> Create(CreateAndUpdateEventDto dto)
        {
            var created = await _service.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                created);
        }


        // GET action
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }


        // GET by Id action
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var eventItem = await _service.GetByIdAsync(id);

            return eventItem is null
                ? NotFound()
                : Ok(eventItem);
        }


        // PUT action
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, CreateAndUpdateEventDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);

            return updated is null
                ? NotFound()
                : Ok(updated);
        }


        // DELETE action
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);

            return deleted
                ? NoContent()
                : NotFound();
        }


        // RESERVE TICKER
        [HttpPost("{id:guid}/reserve")]
        public async Task<IActionResult> ReserveTicket(Guid id)
        {
            var updated = await _service.ReserveTicketAsync(id);

            return updated is null
                ? NotFound()
                : Ok(updated);
        }

        // CANCEL RESERVATION
        [HttpPost("{id:guid}/cancel")]
        public async Task<IActionResult> CancelReservation(Guid id)
        {
            var updated = await _service.CancelReservationAsync(id);

            return updated is null
                ? NotFound()
                : Ok(updated);
        }
    }
}
