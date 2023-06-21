using PdxSlots.API.Dtos.GameMaths;
using PdxSlots.API.Dtos.ZipFileUpload;
using PdxSlots.API.Dtos.ZippedFile;

namespace PdxSlots.API.Services.Interfaces
{
    public interface IZipFileUploadService
    {
        Task<ZippedFileGetDto> UpdateZippedFile(int id, ZippedFilePutDto zippedFilePutDto);
        Task<ZipFileUploadGetDto> GetZipFileUpload(int id);
    }
}
