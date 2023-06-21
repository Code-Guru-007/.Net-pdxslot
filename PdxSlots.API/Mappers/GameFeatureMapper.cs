using AutoMapper;

namespace PdxSlots.API.Mappers
{
    public class GameFeatureMapper : Profile
    {
        public GameFeatureMapper()
        {
            CreateMap<Dtos.GameFeatures.GameFeaturePostDto, Models.GameFeature>();

            CreateMap<Dtos.GameFeatures.GameFeaturePutDto, Models.GameFeature>();

            CreateMap<Models.GameFeature, Dtos.GameFeatures.GameFeatureGetDto>();
        }
    }
}
