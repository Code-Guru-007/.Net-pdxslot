using System;
using System.Collections.Generic;

namespace PdxSlots.API.Models
{
    public partial class ZippedFile
    {
        public ZippedFile()
        {
            PeriodicJobZippedFiles = new HashSet<PeriodicJobZippedFile>();
        }

        public int Id { get; set; }
        public int ZipFileUploadId { get; set; }
        public string LocalFilePath { get; set; }
        public string FileUrl { get; set; }
        public string Hash { get; set; }
        public bool HashCheck { get; set; }
        public DateTime Created { get; set; }

        public virtual ZipFileUpload ZipFileUpload { get; set; }
        public virtual ICollection<PeriodicJobZippedFile> PeriodicJobZippedFiles { get; set; }
    }
}
