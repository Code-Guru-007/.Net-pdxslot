namespace PdxSlots.API.Dtos.ZippedFile
{
    public class ZippedFileGetDto
    {
        public int Id { get; set; }
        public int ZipFileUploadId { get; set; }
        public string LocalFilePath { get; set; }
        public string FileUrl { get; set; }
        public string Hash { get; set; }
        public DateTime Created { get; set; }
        public bool HashCheck { get; set; }
    }
}
