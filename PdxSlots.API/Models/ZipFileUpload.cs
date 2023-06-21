using System;
using System.Collections.Generic;

namespace PdxSlots.API.Models
{
    public partial class ZipFileUpload
    {
        public ZipFileUpload()
        {
            GameDesktopZipFileUploads = new HashSet<Game>();
            GameMaths = new HashSet<GameMath>();
            GameMobileZipFileUploads = new HashSet<Game>();
            ZippedFiles = new HashSet<ZippedFile>();
        }

        public int Id { get; set; }
        public string BlobFileUrl { get; set; }
        public string FileName { get; set; }
        public int UserId { get; set; }
        public DateTime Created { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<Game> GameDesktopZipFileUploads { get; set; }
        public virtual ICollection<GameMath> GameMaths { get; set; }
        public virtual ICollection<Game> GameMobileZipFileUploads { get; set; }
        public virtual ICollection<ZippedFile> ZippedFiles { get; set; }
    }
}
