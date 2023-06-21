using Newtonsoft.Json;

namespace PdxSlots.API.Dtos.Games
{
    public class SpinGameGetDto
    {
        [JsonProperty("denom")]
        public decimal Denom { get; set; }
        [JsonProperty("wager")]
        public decimal Wager { get; set; }
        [JsonProperty("nonwager_purchase")]
        public decimal NonWagerPurchase { get; set; }
        [JsonProperty("commission")]
        public decimal Comission { get; set; }
        [JsonProperty("progressive_jackpot")]
        public decimal ProgressiveJackpot { get; set; }
        [JsonProperty("progressive_contribution")]
        public decimal ProgressiveContribution { get; set; }
        [JsonProperty("win")]
        public decimal Win { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("current_balance")]
        public decimal CurrentBalance { get; set; }
        [JsonProperty("game_result")]
        public object GameResult { get; set; }
    }
}
