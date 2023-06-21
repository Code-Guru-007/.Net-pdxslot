using Newtonsoft.Json;

namespace PdxSlots.IGPClient.Dtos
{
    public class StartGameCycleDto
    {
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("approval_id")]
        public int ApprovalId { get; set; }
        [JsonProperty("transaction_round")]
        public int TransactionRound { get; set; }
        [JsonProperty("funds_start")]
        public decimal FundsStart { get; set; }
    }
}
