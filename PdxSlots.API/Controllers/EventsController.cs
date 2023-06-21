using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PdxSlots.API.Dtos.Events;
using PdxSlots.API.Dtos.Games;
using PdxSlots.API.Dtos.IGCUserGaf;
using PdxSlots.API.Services.Interfaces;

namespace PdxSlots.API.Controllers
{
    [ApiController]
    [Route("/api/v1/[controller]")]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private IEventService _eventsService;

        public EventsController(IEventService eventService)
        {
            _eventsService = eventService;
        }

        [HttpGet]
        public async Task<IEnumerable<EventGetDto>> GetEvents(DateTime start, DateTime? end)
        {
            return await _eventsService.GetEvents(start, end);
        }

        [HttpPost]
        public async Task<EventGetDto> CreateEvent([FromBody] EventPostDto eventPostDto)
        {
            return await _eventsService.CreateEvent(eventPostDto);
        }
    }
}