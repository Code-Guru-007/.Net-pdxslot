using PdxSlots.API.Dtos.GameMaths;
using PdxSlots.API.Dtos.Operators;

namespace PdxSlots.API.Services.Interfaces
{
    public interface IOperatorService
    {
        Task<OperatorGetDto> CreateOperator(OperatorPostDto operatorPostDto);
        Task<OperatorGetDto> EditOperator(int id, OperatorPutDto operatorPutDto);
        Task<IEnumerable<OperatorGetDto>> GetOperators();
    }
}
