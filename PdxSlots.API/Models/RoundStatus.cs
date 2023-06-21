using System;
using System.Collections.Generic;

namespace PdxSlots.API.Models
{
    public partial class RoundStatus
    {
        public RoundStatus()
        {
            Rounds = new HashSet<Round>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Round> Rounds { get; set; }
    }
}
