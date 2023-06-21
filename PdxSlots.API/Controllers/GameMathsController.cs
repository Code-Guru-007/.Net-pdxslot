using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PdxSlots.API.Dtos.GameMaths;
using PdxSlots.API.Dtos.Games;
using PdxSlots.API.Services.Interfaces;

namespace PdxSlots.API.Controllers
{
    [ApiController]
    [Route("/api/v1/[controller]")]
    [Authorize]
    public class GameMathsController : ControllerBase
    {
        private IGameMathsService _gameMathsService;

        public GameMathsController(IGameMathsService gameMathsService)
        {
            _gameMathsService = gameMathsService;
        }
        
        [HttpPost]
        public async Task<GameMathsGetDto> CreateGameMath([FromBody] GameMathPostDto gameMathPostDto)
        {
            return await _gameMathsService.CreateGameMath(gameMathPostDto);
        }
        
        [HttpGet("{id}")]
        public async Task<GameMathsGetDto> GetGameMath([FromRoute] int id)
        {
            return await _gameMathsService.GetGameMath(id);
        }
        
        [HttpPut("{id}")]
        public async Task<GameMathsGetDto> EditGame([FromRoute] int id, [FromBody] GameMathPutDto gameMathPutDto)
        {
            return await _gameMathsService.EditGameMath(id, gameMathPutDto);
        }        
    }
}