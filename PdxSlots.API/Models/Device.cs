using System;
using System.Collections.Generic;

namespace PdxSlots.API.Models
{
    public partial class Device
    {
        public Device()
        {
            Events = new HashSet<Event>();
            Rounds = new HashSet<Round>();
        }

        public int Id { get; set; }
        public int? UserId { get; set; }
        public string IpAddress { get; set; }
        public string OperatingSystem { get; set; }
        public string Browser { get; set; }
        public bool Mobile { get; set; }
        public DateTime Created { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<Round> Rounds { get; set; }
    }
}
