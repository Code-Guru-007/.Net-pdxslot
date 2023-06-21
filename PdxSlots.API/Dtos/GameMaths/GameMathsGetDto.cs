using PdxSlots.API.Dtos.Operators;
using PdxSlots.API.Dtos.ZipFileUpload;

namespace PdxSlots.API.Dtos.GameMaths
{
    public class GameMathsGetDto
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public string ExternalGameId { get; set; }
        public string ExternalOperatorId { get; set; }
        public decimal MaxBet { get; set; }
        public decimal MaxLiability { get; set; }
        public string MathFileUrl { get; set; }
        public string Bets { get; set; }
        public string PayTable { get; set; }
        public string Denominations { get; set; }
        public DateTime AvailableDate { get; set; }
        public DateTime? EndDate { get; set; }
        public OperatorGetDto Operator { get; set; }
        public bool Active { get; set; }
        public ZipFileUploadGetDto MathFileUpload { get; set; }
    }
}
