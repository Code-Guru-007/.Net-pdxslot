using AutoMapper;

namespace PdxSlots.API.Mappers
{
    public class GameMapper : Profile
    {
        public GameMapper()
        {
            CreateMap<Dtos.Games.GamePostDto, Models.Game>()
                .ForMember(src => src.ExternalGameId, dest => dest.MapFrom(x => x.ExternalGameId))
                .ForMember(src => src.DesktopFileUrl, dest => dest.MapFrom(x => x.DesktopFileUrl))
                .ForMember(src => src.MobileFileUrl, dest => dest.MapFrom(x => x.MobileFileUrl));

            CreateMap<Dtos.Games.GamePutDto, Models.Game>()
                .ForMember(src => src.ExternalGameId, dest => dest.MapFrom(x => x.ExternalGameId))
                .ForMember(src => src.DesktopFileUrl, dest => dest.MapFrom(x => x.DesktopFileUrl))
                .ForMember(src => src.MobileFileUrl, dest => dest.MapFrom(x => x.MobileFileUrl));

            CreateMap<Models.Game, Dtos.Games.GameGetDto>();

            // game math

            CreateMap<IGPClient.Dtos.GameMathDto, Dtos.Games.SpinGameGetDto>()
                .ForMember(src => src.GameResult, dest => dest.MapFrom(x => x.RawBody))
                .ForMember(src => src.CurrentBalance, dest => dest.Ignore());

            CreateMap<Models.GameMath, Dtos.GameMaths.GameMathsGetDto>();

            CreateMap<Dtos.GameMaths.GameMathPostDto, Models.GameMath>();
            CreateMap<Dtos.GameMaths.GameMathPutDto, Models.GameMath>();
        }
    }
}
