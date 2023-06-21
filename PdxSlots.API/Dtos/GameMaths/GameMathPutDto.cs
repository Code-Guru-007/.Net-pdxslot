using PdxSlots.API.Dtos.Operators;

namespace PdxSlots.API.Dtos.GameMaths
{
    public class GameMathPutDto
    {
        public decimal MaxBet { get; set; }
        public decimal MaxLiability { get; set; }
        public string MathFileUrl { get; set; }
        public string Bets { get; set; }
        public string PayTable { get; set; }
        public string Denominations { get; set; }
        public DateTime AvailableDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool Active { get; set; }
    }
}
