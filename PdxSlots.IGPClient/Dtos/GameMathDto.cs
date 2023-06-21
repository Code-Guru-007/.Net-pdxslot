using Newtonsoft.Json;

namespace PdxSlots.IGPClient.Dtos
{
    public class GameMathDto : BaseDto
    {
        [JsonProperty("denom")]
        public decimal Denom { get; set; }
        [JsonProperty("wager")]
        public decimal Wager { get; set; }
        [JsonProperty("nonwager_purchase")]
        public int NonWagerPurchase { get; set; }
        [JsonProperty("commission")]
        public int Comission { get; set; }
        [JsonProperty("progressive_jackpot")]
        public int ProgressiveJackpot { get; set; }
        [JsonProperty("progressive_contribution")]
        public int ProgressiveContribution { get; set; }
        [JsonProperty("win")]
        public decimal Win { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("R1")]
        public string R1 { get; set; }
        [JsonProperty("R2")]
        public string R2 { get; set; }
        [JsonProperty("R3")]
        public string R3 { get; set; }
        [JsonProperty("R4")]
        public string R4 { get; set; }
        [JsonProperty("R5")]
        public string R5 { get; set; }
        [JsonProperty("PP1")]
        public decimal PP1 { get; set; }
        [JsonProperty("PP2")]
        public decimal PP2 { get; set; }
        [JsonProperty("PP3")]
        public decimal PP3 { get; set; }
        [JsonProperty("PP4")]
        public decimal PP4 { get; set; }
        [JsonProperty("PP5")]
        public decimal PP5 { get; set; }
        [JsonProperty("PP6")]
        public decimal PP6 { get; set; }
        [JsonProperty("PP7")]
        public decimal PP7 { get; set; }
        [JsonProperty("PP8")]
        public decimal PP8 { get; set; }
        [JsonProperty("PP9")]
        public decimal PP9 { get; set; }
        [JsonProperty("PP10")]
        public decimal PP10 { get; set; }
        [JsonProperty("PP11")]
        public decimal PP11 { get; set; }
        [JsonProperty("PP12")]
        public decimal PP12 { get; set; }
        [JsonProperty("PP13")]
        public decimal PP13 { get; set; }
        [JsonProperty("PP14")]
        public decimal PP14 { get; set; }
        [JsonProperty("PP15")]
        public decimal PP15 { get; set; }
        [JsonProperty("PP")]
        public IList<decimal> PP { get; set; }
    }
}
