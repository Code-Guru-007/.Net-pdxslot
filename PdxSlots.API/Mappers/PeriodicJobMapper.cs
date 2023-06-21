using AutoMapper;
using PdxSlots.API.Dtos.PeriodicJobs;

namespace PdxSlots.API.Mappers
{
    public class PeriodicJobMapper : Profile
    {
        public PeriodicJobMapper()
        {
            CreateMap<Models.PeriodicJob, PeriodicJobGetDto>()
                .ForMember(x => x.ZippedFiles, src => src.MapFrom(x => x.PeriodicJobZippedFiles));

            CreateMap<Models.PeriodicJobZippedFile, PeriodicJobZippedFileGetDto>();
        }
    }
}
