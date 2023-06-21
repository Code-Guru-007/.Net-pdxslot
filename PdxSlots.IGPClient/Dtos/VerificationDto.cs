using Newtonsoft.Json;

namespace PdxSlots.IGPClient.Dtos
{
    public class VerificationDto
    {
        [JsonProperty("vendor_id")]
        public string VendorId { get; set; }
        [JsonProperty("user_id")]
        public string UserId { get; set; }
        [JsonProperty("verification")]
        public string Verification { get; set; }
        [JsonProperty("withdrawable_balance")]
        public decimal WithdrawableBalance { get; set; }
        [JsonProperty("freeplay_balance")]
        public decimal FreeplayBalance { get; set; }
        [JsonProperty("current_balance")]
        public decimal CurrentBalance { get; set; }
    }
}
