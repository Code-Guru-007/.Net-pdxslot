using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PdxSlots.API.Dtos.Games;
using PdxSlots.API.Dtos.IGCUserGaf;
using PdxSlots.API.Services.Interfaces;

namespace PdxSlots.API.Controllers
{
    [ApiController]
    [Route("/api/v1/[controller]")]
    [Authorize]
    public class GafsController : ControllerBase
    {
        private IIgcUserGafService _igcUserGafService;

        public GafsController(IIgcUserGafService igcUserGafService)
        {
            _igcUserGafService = igcUserGafService;
        }

        [HttpGet]
        public async Task<IEnumerable<IgcUserGafGetDto>> GetGafs()
        {
            return await _igcUserGafService.GetUserGafs();
        }

        [HttpPost]
        public async Task<IgcUserGafGetDto> CreateGaf([FromBody] IgcUserGafPostDto  igcUserGafPostDto)
        {
            return await _igcUserGafService.CreateUserGaf(igcUserGafPostDto);
        }

        [HttpPut("{id}")]
        public async Task<IgcUserGafGetDto> UpdateUserGaf([FromRoute] int id, [FromBody] IgcUserGafPutDto igcUserGafPutDto)
        {
            return await _igcUserGafService.UpdateUserGaf(id, igcUserGafPutDto);
        }
        
        [HttpDelete("{id}")]
        public async Task<bool> DeleteGaf([FromRoute] int id)
        {
            return await _igcUserGafService.DeleteUserGaf(id);
        }
    }
}