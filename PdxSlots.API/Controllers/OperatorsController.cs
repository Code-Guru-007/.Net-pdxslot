using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PdxSlots.API.Dtos.Operators;
using PdxSlots.API.Services.Interfaces;

namespace PdxSlots.API.Controllers
{
    [ApiController]
    [Route("/api/v1/[controller]")]
    [Authorize]
    public class OperatorsController : ControllerBase
    {
        private IOperatorService _operatorService;

        public OperatorsController(IOperatorService operatorService)
        {
            _operatorService = operatorService;
        }

        [HttpGet]
        public async Task<IEnumerable<OperatorGetDto>> GetOperators()
        {
            return await _operatorService.GetOperators();
        }

        [HttpPost]
        public async Task<OperatorGetDto> CreateOperator([FromBody] OperatorPostDto operatorPostDto)
        {
            return await _operatorService.CreateOperator(operatorPostDto);
        }

        [HttpPut("{id}")]
        public async Task<OperatorGetDto> EditGame([FromRoute] int id, [FromBody] OperatorPutDto operatorPutDto)
        {
            return await _operatorService.EditOperator(id, operatorPutDto);
        }        
    }
}