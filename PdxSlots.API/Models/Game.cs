using System;
using System.Collections.Generic;

namespace PdxSlots.API.Models
{
    public partial class Game
    {
        public Game()
        {
            Events = new HashSet<Event>();
            GameFeatures = new HashSet<GameFeature>();
            GameMaths = new HashSet<GameMath>();
            Rounds = new HashSet<Round>();
        }

        public int Id { get; set; }
        public string ExternalGameId { get; set; }
        public string DesktopFileUrl { get; set; }
        public int? DesktopZipFileUploadId { get; set; }
        public string MobileFileUrl { get; set; }
        public int? MobileZipFileUploadId { get; set; }
        public bool Active { get; set; }

        public virtual ZipFileUpload DesktopZipFileUpload { get; set; }
        public virtual ZipFileUpload MobileZipFileUpload { get; set; }
        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<GameFeature> GameFeatures { get; set; }
        public virtual ICollection<GameMath> GameMaths { get; set; }
        public virtual ICollection<Round> Rounds { get; set; }
    }
}
