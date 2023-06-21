using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PdxSlots.API.Dtos.Games;
using PdxSlots.API.Dtos.Rounds;
using PdxSlots.API.Services.Interfaces;
using System.Collections.Generic;

namespace PdxSlots.API.Controllers
{
    [ApiController]
    [Route("/api/v1/[controller]")]
    public class RoundsController : ControllerBase
    {
        private IRoundService _roundService;

        public RoundsController(IRoundService roundService)
        {
            _roundService = roundService;
        }
        
        [HttpGet]
        [AllowAnonymous]
        public async Task<IEnumerable<RoundGetDto>> GetRounds(DateTime start, DateTime? end)
        {
            return await _roundService.GetRounds(start, end);
        }

        [HttpGet("stuck")]
        [AllowAnonymous]
        public async Task<IEnumerable<RoundGetDto>> GetStuckRounds(DateTime start, DateTime? end)
        {
            return await _roundService.GetStuckRounds(start, end);
        }

        [HttpPut("void/{id}")]
        [AllowAnonymous]
        public async Task<RoundGetDto> VoidRound([FromRoute] int id)
        {
            return await _roundService.VoidRound(id);
        }
    }
}