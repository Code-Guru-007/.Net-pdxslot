using AutoMapper;

namespace PdxSlots.API.Mappers
{
    public class RoundMapper : Profile
    {
        public RoundMapper()
        {
            CreateMap<Models.Round, Dtos.Rounds.RoundGetDto>()
                .ForMember(dest => dest.Status, opts => opts.MapFrom(src => src.Status))
                .ForMember(dest => dest.Game, opts => opts.MapFrom(src => src.Game))
                .ForMember(dest => dest.Operator, opts => opts.MapFrom(src => src.Operator))
                .ForMember(dest => dest.Device, opts => opts.MapFrom(src => src.Device));

            CreateMap<Models.RoundStatus, Dtos.StatusGetDto>();
        }
    }
}
