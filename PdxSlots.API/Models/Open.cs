using System;
using System.Collections.Generic;

namespace PdxSlots.API.Models
{
    public partial class Open
    {
        public int Id { get; set; }
        public string OperatorId { get; set; }
        public string UserId { get; set; }
        public string ExternalGameId { get; set; }
        public string IpAddress { get; set; }
        public string OperatingSystem { get; set; }
        public string Browser { get; set; }
        public bool Mobile { get; set; }
        public DateTime Created { get; set; }
    }
}
