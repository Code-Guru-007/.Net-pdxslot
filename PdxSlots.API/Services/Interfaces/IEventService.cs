using Azure.Storage.Blobs;
using PdxSlots.API.Dtos.Events;

namespace PdxSlots.API.Services.Interfaces
{
    public interface IEventService
    {
        Task<EventGetDto> CreateEvent(EventPostDto eventPostDto);
        Task<EventGetDto> CreateEvent(string name, string description, int? gameId = null, int? gameMathId = null, int? operatorId = null, int? roundId = null);
        Task<IEnumerable<EventGetDto>> GetEvents(DateTime start, DateTime? end);
    }
}
