using AutoMapper;
namespace PdxSlots.API.Mappers
{
    public class IgcUserGafMapper : Profile
    {
        public IgcUserGafMapper()
        {
            CreateMap<Models.IgcuserGaf, Dtos.IGCUserGaf.IgcUserGafGetDto>();

            CreateMap<Dtos.IGCUserGaf.IgcUserGafPutDto, Models.IgcuserGaf>();
            CreateMap<Dtos.IGCUserGaf.IgcUserGafPostDto, Models.IgcuserGaf>();
        }
    }
}
