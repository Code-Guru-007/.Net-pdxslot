using System;
using System.Collections.Generic;

namespace PdxSlots.API.Models
{
    public partial class Event
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? DeviceId { get; set; }
        public int? GameId { get; set; }
        public int? GameMathId { get; set; }
        public int? OperatorId { get; set; }
        public int? RoundId { get; set; }
        public int? PeriodicJobId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }

        public virtual Device Device { get; set; }
        public virtual Game Game { get; set; }
        public virtual GameMath GameMath { get; set; }
        public virtual Operator Operator { get; set; }
        public virtual PeriodicJob PeriodicJob { get; set; }
        public virtual Round Round { get; set; }
        public virtual User User { get; set; }
    }
}
