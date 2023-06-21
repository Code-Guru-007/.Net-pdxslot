using PdxSlots.API.Dtos.GameMaths;

namespace PdxSlots.API.Dtos.Games
{
    public class GamePostDto
    {
        public string ExternalGameId { get; set; }
        public string DesktopFileUrl { get; set; }
        public string MobileFileUrl { get; set; }
    }
}
