using AutoMapper;

namespace PdxSlots.API.Mappers
{
    public class OperatorMapper : Profile
    {
        public OperatorMapper()
        {
            CreateMap<Dtos.Operators.OperatorPostDto, Models.Operator>();
            CreateMap<Dtos.Operators.OperatorPutDto, Models.Operator>();

            CreateMap<Models.Operator, Dtos.Operators.OperatorGetDto>();
        }
    }
}
