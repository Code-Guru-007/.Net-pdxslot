using System;
using System.Collections.Generic;

namespace PdxSlots.API.Models
{
    public partial class GameMath
    {
        public GameMath()
        {
            Events = new HashSet<Event>();
            GameFeatures = new HashSet<GameFeature>();
        }

        public int Id { get; set; }
        public int GameId { get; set; }
        public int OperatorId { get; set; }
        public string PayTable { get; set; }
        public string ExternalGameId { get; set; }
        public string ExternalOperatorId { get; set; }
        public decimal MaxBet { get; set; }
        public decimal MaxLiability { get; set; }
        public string Bets { get; set; }
        public string Denominations { get; set; }
        public string MathFileUrl { get; set; }
        public int? MathFileUploadId { get; set; }
        public DateTime AvailableDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Active { get; set; }

        public virtual Game Game { get; set; }
        public virtual ZipFileUpload MathFileUpload { get; set; }
        public virtual Operator Operator { get; set; }
        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<GameFeature> GameFeatures { get; set; }
    }
}
