using PdxSlots.API.Dtos.Devices;
using PdxSlots.API.Dtos.Games;
using PdxSlots.API.Dtos.Operators;

namespace PdxSlots.API.Dtos.Rounds
{
    public class RoundGetDto
    {
        public int Id { get; set; }
        public string SessionId { get; set; }
        public string ExternalOperatorId { get; set; }
        public string ExternalGameId { get; set; }
        public int OperatorId { get; set; }
        public int GameId { get; set; }
        public decimal Wager { get; set; }
        public decimal Denomination { get; set; }
        public decimal WagerCurrency { get; set; }
        public decimal Win { get; set; }
        public decimal WinCurrency { get; set; }
        public string PayTable { get; set; }
        public string WalletApprovalId { get; set; }
        public string WalletRoundId { get; set; }
        public string UserId { get; set; }
        public int DeviceId { get; set; }
        public int StatusId { get; set; }
        public string GameResult { get; set; }
        public decimal FundsStart { get; set; }
        public decimal? FundsEnd { get; set; }
        public string Gaf { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        public DeviceGetDto Device { get; set; }
        public GameGetDto Game { get; set; }
        public OperatorGetDto Operator { get; set; }
        public StatusGetDto Status { get; set; }
    }
}
