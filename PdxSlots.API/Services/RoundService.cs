using AutoMapper;
using AutoWrapper.Wrappers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PdxSlots.API.Common;
using PdxSlots.API.Data;
using PdxSlots.API.Dtos.Games;
using PdxSlots.API.Dtos.Rounds;
using PdxSlots.API.Services.Interfaces;
using PdxSlots.IGPClient.Services;
using System.Net;
using Wangkanai.Detection.Services;

namespace PdxSlots.API.Services
{
    public class RoundService : IRoundService
    {
        private readonly PdxSlotsContext _pdxSlotsContext;
        private readonly IMapper _mapper;
        private readonly IGCPClientService _iGCPClientService;
        private readonly IEventService _eventService;

        public RoundService(PdxSlotsContext pdxSlotsContext, IMapper mapper, IGCPClientService iGCPClientService, IEventService eventService)
        {
            _pdxSlotsContext = pdxSlotsContext;
            _mapper = mapper;
            _iGCPClientService = iGCPClientService;
            _eventService = eventService;
        }

        public async Task<IEnumerable<RoundGetDto>> GetRounds(DateTime start, DateTime? end)
        {
            var endDate = DateTime.UtcNow;

            if(end.HasValue) endDate = end.Value;

            var rounds = await _pdxSlotsContext
                .Rounds
                .Include(x=>x.Game)
                .Include(x=>x.Operator)
                .Include(x=>x.Device)
                .Include(x=>x.Status)
                .Where(x => x.Updated > start && x.Updated <= endDate)
                .ToListAsync();

            return _mapper.Map<IEnumerable<RoundGetDto>>(rounds);
        }

        public async Task<IEnumerable<RoundGetDto>> GetStuckRounds(DateTime start, DateTime? end)
        {
            var endDate = DateTime.UtcNow;

            if (end.HasValue) endDate = end.Value;

            var rounds = await _pdxSlotsContext
                .Rounds
                .Include(x => x.Game)
                .Include(x => x.Operator)
                .Include(x => x.Device)
                .Include(x => x.Status)
                .Where(x => x.Updated > start && x.Updated <= endDate && x.StatusId == (int)Models.Enums.RoundStatus.Open)
                .ToListAsync();

            return _mapper.Map<IEnumerable<RoundGetDto>>(rounds);
        }

        public async Task<RoundGetDto> VoidRound(int id)
        {
            var roundToVoid = await _pdxSlotsContext.Rounds.FirstOrDefaultAsync(x => x.Id == id && x.StatusId == (int)Models.Enums.RoundStatus.Open);

            if (roundToVoid == null)
            {
                await _eventService.CreateEvent($"{Constants.EventNames.Error}", $"Void Round - Round id: {id}, " +
                    $"Error: [Stuck round not found.]");
                throw new ApiException("Stuck round not found", (int)HttpStatusCode.NotFound);
            }

                var voidResult = await _iGCPClientService.VoidTransaction(roundToVoid.UserId, roundToVoid?.Operator?.VendorId, roundToVoid.SessionId);

            roundToVoid.StatusId = (int)Models.Enums.RoundStatus.Voided;
            roundToVoid.FundsEnd = voidResult.FundsEnd;

            await _pdxSlotsContext.SaveChangesAsync();
            
            return _mapper.Map<RoundGetDto>(roundToVoid); 
        }
    }
}
