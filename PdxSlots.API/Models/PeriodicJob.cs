using System;
using System.Collections.Generic;

namespace PdxSlots.API.Models
{
    public partial class PeriodicJob
    {
        public PeriodicJob()
        {
            Events = new HashSet<Event>();
            PeriodicJobZippedFiles = new HashSet<PeriodicJobZippedFile>();
        }

        public int Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }

        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<PeriodicJobZippedFile> PeriodicJobZippedFiles { get; set; }
    }
}
