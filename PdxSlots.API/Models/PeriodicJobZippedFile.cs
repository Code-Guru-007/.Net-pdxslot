using System;
using System.Collections.Generic;

namespace PdxSlots.API.Models
{
    public partial class PeriodicJobZippedFile
    {
        public int Id { get; set; }
        public int PeriodicJobId { get; set; }
        public int ZippedFileId { get; set; }
        public string OriginalHash { get; set; }
        public string CurrentHash { get; set; }
        public bool HashEquals { get; set; }
        public bool HashCheck { get; set; }
        public DateTime Created { get; set; }

        public virtual PeriodicJob PeriodicJob { get; set; }
        public virtual ZippedFile ZippedFile { get; set; }
    }
}
