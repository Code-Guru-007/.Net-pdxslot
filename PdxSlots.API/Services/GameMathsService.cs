using AutoMapper;
using AutoWrapper.Wrappers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PdxSlots.API.Common;
using PdxSlots.API.Data;
using PdxSlots.API.Dtos.GameMaths;
using PdxSlots.API.Dtos.Games;
using PdxSlots.API.Services.Interfaces;
using PdxSlots.IGPClient.Services;
using System.Net;
using System.Reflection;
using Wangkanai.Detection.Services;

namespace PdxSlots.API.Services
{
    public class GameMathsService : IGameMathsService
    {
        private readonly PdxSlotsContext _pdxSlotsContext;
        private readonly IMapper _mapper;
        private readonly IEventService _eventService;

        public GameMathsService(PdxSlotsContext pdxSlotsContext, IMapper mapper, IEventService eventService)
        {
            _pdxSlotsContext = pdxSlotsContext;
            _mapper = mapper;
            _eventService = eventService;
        }

        public async Task<GameMathsGetDto> CreateGameMath(GameMathPostDto gameMathPostDto)
        {
            VerifyGameMathDto(gameMathPostDto, out Models.Operator @operator, out Models.Game game);

            var gameMath = _mapper.Map<Models.GameMath>(gameMathPostDto);
            gameMath.ExternalGameId = game.ExternalGameId;
            gameMath.ExternalOperatorId = @operator.ExternalOperatorId;

            await _pdxSlotsContext.GameMaths.AddAsync(gameMath);
            await _pdxSlotsContext.SaveChangesAsync();

            return _mapper.Map<GameMathsGetDto>(gameMath);
        }

        private void VerifyGameMathDto(GameMathPostDto gameMathPostDto, out Models.Operator @operator, out Models.Game game)
        {
            @operator = _pdxSlotsContext.Operators.FirstOrDefault(x => x.Id == gameMathPostDto.OperatorId);
            if (@operator == null)
            {
                throw new ApiException("Operator not found.", (int)HttpStatusCode.NotFound);
            }

            game = _pdxSlotsContext.Games.FirstOrDefault(x => x.Id == gameMathPostDto.GameId);
            if (game == null)
            {
                throw new ApiException("Game not found.", (int)HttpStatusCode.NotFound);
            }
        }

        public async Task<GameMathsGetDto> EditGameMath(int id, GameMathPutDto gameMathPutDto)
        {
            var gameMath = await _pdxSlotsContext.GameMaths.FirstOrDefaultAsync(x => x.Id == id);
            if (gameMath == null)
            {
                await _eventService.CreateEvent($"{Constants.EventNames.Error}", $"Updating game math - " +
                    $"Game id : {gameMath.GameId} , Game math id : {gameMath.Id} Error: [Game Math not found.]");
                throw new ApiException("Game Math not found.", (int)HttpStatusCode.NotFound);
            }

            await _eventService.CreateEvent($"{Constants.EventNames.Update}", $"Updating game math - " +
                $"Game id : {gameMath.GameId} , Game math id : {gameMath.Id} Updated: [{gameMath.ToComparisonEventDescription(gameMathPutDto)}]", gameMath.GameId, gameMath.Id);

            _mapper.Map(gameMathPutDto, gameMath);

            await _pdxSlotsContext.SaveChangesAsync();

            return _mapper.Map<GameMathsGetDto>(gameMath);
        }

        public async Task<GameMathsGetDto> GetGameMath(int id)
        {
            var gameMath = await _pdxSlotsContext
                .GameMaths
                .Include(x=>x.Operator)
                .Include(x=>x.MathFileUpload)
                    .ThenInclude(x=>x.ZippedFiles)
                .Include(x=>x.MathFileUpload)
                .FirstOrDefaultAsync(x => x.Id == id);

            return _mapper.Map<GameMathsGetDto>(gameMath);
        }
    }
}
