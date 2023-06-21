using AutoMapper;

namespace PdxSlots.API.Mappers
{
    public class EventMapper : Profile
    {
        public EventMapper()
        {
            CreateMap<Models.Event, Dtos.Events.EventGetDto>()
                .ForMember(dest => dest.Device, src => src.MapFrom(x=>x.Device))
                .ForMember(dest => dest.Game, src => src.MapFrom(x=>x.Game))
                .ForMember(dest => dest.GameMath, src => src.MapFrom(x=>x.GameMath))
                .ForMember(dest => dest.Operator, src => src.MapFrom(x=>x.Operator))
                .ForMember(dest => dest.Round, src => src.MapFrom(x=>x.Round))
                .ForMember(dest => dest.User, src => src.MapFrom(x=>x.User));

            CreateMap<Dtos.Events.EventPostDto, Models.Event>();
        }
    }
}
