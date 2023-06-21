using System;
using System.Collections.Generic;

namespace PdxSlots.API.Models
{
    public partial class Round
    {
        public Round()
        {
            Events = new HashSet<Event>();
        }

        public int Id { get; set; }
        public string SessionId { get; set; }
        public string ExternalOperatorId { get; set; }
        public string ExternalGameId { get; set; }
        public int OperatorId { get; set; }
        public int GameId { get; set; }
        public decimal Wager { get; set; }
        public decimal Denomination { get; set; }
        public decimal WagerCurrency { get; set; }
        public string WagerDetail { get; set; }
        public decimal NonWager { get; set; }
        public string NonWagerDetail { get; set; }
        public decimal ProgressiveWin { get; set; }
        public decimal ProgressiveWinCont { get; set; }
        public decimal IncentiveWager { get; set; }
        public string IncentiveWagerDetail { get; set; }
        public decimal IncentiveWin { get; set; }
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

        public virtual Device Device { get; set; }
        public virtual Game Game { get; set; }
        public virtual Operator Operator { get; set; }
        public virtual RoundStatus Status { get; set; }
        public virtual ICollection<Event> Events { get; set; }
    }
}
