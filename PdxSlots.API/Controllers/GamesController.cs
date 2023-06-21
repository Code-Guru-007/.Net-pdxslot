using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PdxSlots.API.Dtos.GameFeatures;
using PdxSlots.API.Dtos.Games;
using PdxSlots.API.Services.Interfaces;
using System.Collections.Generic;

namespace PdxSlots.API.Controllers
{
    [ApiController]
    [Route("/api/v1/[controller]")]
    [Authorize]
    public class GamesController : ControllerBase
    {
        private IGamesService _gamesService;

        public GamesController(IGamesService gamesService)
        {
            _gamesService = gamesService;
        }

        [HttpGet]
        public async Task<IEnumerable<GameGetDto>> GetGames()
        {
            return await _gamesService.GetGames();
        }

        [HttpGet("{id}")]
        public async Task<GameGetDto> GetGame([FromRoute] int id)
        {
            return await _gamesService.GetGame(id);
        }

        [HttpPost]
        public async Task<GameGetDto> CreateGame([FromBody] GamePostDto gamePostDto)
        {
            return await _gamesService.CreateGame(gamePostDto);
        }

        [HttpPost("{id}/content/mobile")]
        public async Task<GameGetDto> CreateMobileGameContent([FromRoute] int id, [FromForm] IFormFile zipContent)
        {
            return await _gamesService.CreateGameContent(id, zipContent, true);
        }

        [HttpPost("{id}/content/desktop")]
        public async Task<GameGetDto> CreateDesktopGameContent([FromRoute] int id, [FromForm] IFormFile zipContent)
        {
            return await _gamesService.CreateGameContent(id, zipContent, false);
        }

        [HttpPost("{id}/maths/{gameMathId}")]
        public async Task<GameGetDto> CreateMobileGameContent([FromRoute] int id,[FromRoute] int gameMathId, [FromForm] IFormFile zipContent)
        {
            return await _gamesService.CreateGameMathContent(id, gameMathId, zipContent);
        }

        [HttpPut("{id}")]
        public async Task<GameGetDto> EditGame([FromRoute] int id, [FromBody] GamePutDto gamePutDto)
        {
            return await _gamesService.EditGame(id, gamePutDto);
        }

        [HttpGet("open")]
        [AllowAnonymous]
        public async Task<IActionResult> OpenGame(string user_id, string operator_id, string game_id, string session_id, bool mobile_override = false)
        {
            var returnUrl = await _gamesService.OpenGame(user_id, operator_id, game_id, session_id, mobile_override);

            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);

            return BadRequest(new { status = "error" });
        }

        [HttpGet("open/link")]
        public async Task<string> OpenGameLink(string user_id, string operator_id, string game_id, string session_id, bool mobile_override = false)
        {
            return await _gamesService.OpenGame(user_id, operator_id, game_id, session_id, mobile_override);
        }

        [HttpGet("initialize")]
        [AllowAnonymous]
        public async Task<InitializeGameGetDto> InitializeGame(string user_id, string operator_id, string game_id, string session_id)
        {
            return await _gamesService.InitializeGame(user_id, operator_id, game_id, session_id);
        }

        [HttpGet("spin")]
        [AllowAnonymous]
        public async Task<SpinGameGetDto> Spin(string user_id, string operator_id, string game_id, string session_id, decimal denom, decimal wager)
        {
            return await _gamesService.SpinGame(user_id, operator_id, game_id, session_id, denom, wager);
        }

        [HttpGet("features")]
        public async Task<IEnumerable<GameFeatureGetDto>> GetFeatures()
        {
            return await _gamesService.GetGameFeatures();
        }
        
        [HttpPost("features")]
        public async Task<GameFeatureGetDto> CreateFeature([FromBody] GameFeaturePostDto gameFeaturePostDto)
        {
            return await _gamesService.CreateGameFeature(gameFeaturePostDto);
        }
        
        [HttpPut("features/{id}")]
        public async Task<GameFeatureGetDto> UpdateFeature([FromRoute] int id, [FromBody] GameFeaturePutDto gameFeaturePutDto)
        {
            return await _gamesService.UpdateGameFeature(id, gameFeaturePutDto);
        }
        
        [HttpDelete("features/{id}")]
        public async Task<bool> DeleteFeature([FromRoute] int id)
        {
            return await _gamesService.DeleteGameFeature(id);
        }
    }
}