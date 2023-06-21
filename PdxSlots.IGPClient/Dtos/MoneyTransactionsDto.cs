using Newtonsoft.Json;

namespace PdxSlots.IGPClient.Dtos
{
    public class MoneyTransactionsDto
    {
        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
