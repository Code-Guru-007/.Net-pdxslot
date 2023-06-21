using PdxSlots.API.Dtos.GameMaths;

namespace PdxSlots.API.Dtos.Games
{
    public class GameGetDto
    {
        public int Id { get; set; }
        public string ExternalGameId { get; set; }
        public string DesktopFileUrl { get; set; }
        public string MobileFileUrl { get; set; }
        public bool Active { get; set; }
        public ZipFileUpload.ZipFileUploadGetDto DesktopZipFileUpload { get; set; }
        public ZipFileUpload.ZipFileUploadGetDto MobileZipFileUpload { get; set; }

        public IEnumerable<GameMathsGetDto> GameMaths { get; set; }
    }
}
