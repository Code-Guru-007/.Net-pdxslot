using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PdxSlots.API.Dtos.GameMaths;
using PdxSlots.API.Dtos.Games;
using PdxSlots.API.Dtos.ZipFileUpload;
using PdxSlots.API.Dtos.ZippedFile;
using PdxSlots.API.Services.Interfaces;

namespace PdxSlots.API.Controllers
{
    [ApiController]
    [Route("/api/v1/[controller]")]
    [Authorize]
    public class ZipFileUploadsController : ControllerBase
    {
        private IZipFileUploadService _zipFileUploadService;

        public ZipFileUploadsController(IZipFileUploadService zipFileUploadService)
        {
            _zipFileUploadService = zipFileUploadService;
        }
        
        
        [HttpGet("{id}")]
        public async Task<ZipFileUploadGetDto> GetZipFileUpload([FromRoute] int id)
        {
            return await _zipFileUploadService.GetZipFileUpload(id);
        }
        
        [HttpPut("zip-files/{id}")]
        public async Task<ZippedFileGetDto> EditZippedFile([FromRoute] int id, [FromBody] ZippedFilePutDto zippedFilePutDto)
        {
            return await _zipFileUploadService.UpdateZippedFile(id, zippedFilePutDto);
        }        
    }
}