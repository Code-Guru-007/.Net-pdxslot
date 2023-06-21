using System;
using System.Collections.Generic;

namespace PdxSlots.API.Models
{
    public partial class Operator
    {
        public Operator()
        {
            Events = new HashSet<Event>();
            GameFeatures = new HashSet<GameFeature>();
            GameMaths = new HashSet<GameMath>();
            IgcuserGafs = new HashSet<IgcuserGaf>();
            Rounds = new HashSet<Round>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string ExternalOperatorId { get; set; }
        public string VendorId { get; set; }

        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<GameFeature> GameFeatures { get; set; }
        public virtual ICollection<GameMath> GameMaths { get; set; }
        public virtual ICollection<IgcuserGaf> IgcuserGafs { get; set; }
        public virtual ICollection<Round> Rounds { get; set; }
    }
}
