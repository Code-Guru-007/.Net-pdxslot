using PdxSlots.API.Dtos.ZippedFile;

namespace PdxSlots.API.Dtos.ZipFileUpload
{
    public class ZipFileUploadGetDto
    {
        public int Id { get; set; }
        public string BlobFileUrl { get; set; }
        public string FileName { get; set; }
        public int UserId { get; set; }
        public DateTime Created { get; set; }

        public ICollection<ZippedFileGetDto> ZippedFiles { get; set; }
    }
}
