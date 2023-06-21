using AutoMapper;
using AutoWrapper.Wrappers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PdxSlots.API.Common;
using PdxSlots.API.Data;
using PdxSlots.API.Dtos.GameMaths;
using PdxSlots.API.Dtos.Games;
using PdxSlots.API.Dtos.Operators;
using PdxSlots.API.Services.Interfaces;
using PdxSlots.IGPClient.Services;
using System.Net;
using System.Reflection;
using Wangkanai.Detection.Services;

namespace PdxSlots.API.Services
{
    public class OperatorService : IOperatorService
    {
        private readonly PdxSlotsContext _pdxSlotsContext;
        private readonly IEventService _eventService;
        private readonly IMapper _mapper;

        public OperatorService(PdxSlotsContext pdxSlotsContext, IMapper mapper, IEventService eventService)
        {
            _pdxSlotsContext = pdxSlotsContext;
            _mapper = mapper;
            _eventService = eventService;
        }

        public async Task<OperatorGetDto> CreateOperator(OperatorPostDto operatorPostDto)
        {
            var @operator = _mapper.Map<Models.Operator>(operatorPostDto);

            await _pdxSlotsContext.Operators.AddAsync(@operator);
            await _pdxSlotsContext.SaveChangesAsync();

            return _mapper.Map<OperatorGetDto>(@operator);
        }

        public async Task<OperatorGetDto> EditOperator(int id, OperatorPutDto operatorPutDto)
        {
            var @operator = await _pdxSlotsContext.Operators.FirstOrDefaultAsync(x => x.Id == id);

            if (@operator == null)
            {
                await _eventService.CreateEvent($"{Constants.EventNames.Error}", $"Updating operator - Operator Id: {id}" +
                    $"Error: [Operator not found.]");
                throw new ApiException("Operator not found.", (int)HttpStatusCode.NotFound);
            }

            _mapper.Map(operatorPutDto, @operator);

            await _pdxSlotsContext.SaveChangesAsync();

            return _mapper.Map<OperatorGetDto>(@operator);
        }

        public async Task<IEnumerable<OperatorGetDto>> GetOperators()
        {
            var operators = await _pdxSlotsContext.Operators.ToListAsync();

            return _mapper.Map<IEnumerable<OperatorGetDto>>(operators); 
        }
    }
}
