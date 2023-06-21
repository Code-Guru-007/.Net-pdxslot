using PdxSlots.API.Dtos.Games;
using PdxSlots.API.Dtos.Rounds;

namespace PdxSlots.API.Services.Interfaces
{
    public interface IRoundService
    {
        Task<IEnumerable<RoundGetDto>> GetRounds(DateTime start, DateTime? end);
        Task<IEnumerable<RoundGetDto>> GetStuckRounds(DateTime start, DateTime? end);
        Task<RoundGetDto> VoidRound(int id);
    }
}
