using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PdxSlots.API.Dtos.Games;
using PdxSlots.API.Dtos.PeriodicJobs;
using PdxSlots.API.Services.Interfaces;

namespace PdxSlots.API.Controllers
{
    [ApiController]
    [Route("/api/v1/[controller]")]
    [Authorize]
    public class PeriodicJobsController : ControllerBase
    {
        private IPeriodicJobService _periodicJobService;

        public PeriodicJobsController(IPeriodicJobService periodicJobService)
        {
            _periodicJobService = periodicJobService;
        }


        [HttpPost("start")]
        public async Task<PeriodicJobGetDto> RunJob()
        {
            return await _periodicJobService.StartJob();
        }
        
        [HttpPut("emails")]
        public async Task<PeriodicJobEmailsGetDto> UpdateEmails([FromBody] PeriodicJobEmailsPostDto periodicJobEmailsPostDto)
        {
            return await _periodicJobService.UpdateEmails(periodicJobEmailsPostDto);
        }

        [HttpGet("emails")]
        public async Task<PeriodicJobEmailsGetDto> GetEmails()
        {
            return await _periodicJobService.GetEmails();
        }

        [HttpGet]
        public async Task<IEnumerable<PeriodicJobGetDto>> GetJobs()
        {
            return await _periodicJobService.GetJobs();
        }

    }
}