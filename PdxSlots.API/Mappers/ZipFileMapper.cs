using AutoMapper;
using PdxSlots.API.Dtos.Users;

namespace PdxSlots.API.Mappers
{
    public class ZipFileMapper : Profile
    {
        public ZipFileMapper()
        {
            CreateMap<Models.ZipFileUpload, Dtos.ZipFileUpload.ZipFileUploadGetDto>();

            CreateMap<Models.ZippedFile, Dtos.ZippedFile.ZippedFileGetDto>();

            CreateMap<Dtos.ZippedFile.ZippedFilePutDto, Models.ZippedFile>();
        }
    }
}
