using Newtonsoft.Json;

namespace PdxSlots.IGPClient.Dtos
{
    public class EndMultiGameCycleDto
    {
        [JsonProperty("wager")]
        public decimal Wager { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("game_result")]
        public string GameResult { get; set; }
    }
}
