namespace PdxSlots.API.Dtos.Games
{
    public class InitializeGameGetDto
    {
        public decimal current_balance { get; set; }
        public string status { get; set; }

        public string denominations { get; set; }
        public string bets { get; set; }
    }
}
