using Newtonsoft.Json;

namespace PdxSlots.IGPClient.Dtos
{
    public class EndGameCycleDto
    {
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("funds_end")]
        public decimal FundsEnd { get; set; }
    }
}
