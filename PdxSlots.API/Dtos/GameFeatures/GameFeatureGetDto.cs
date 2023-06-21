namespace PdxSlots.API.Dtos.GameFeatures
{
    public class GameFeatureGetDto
    {
        public int Id { get; set; }
        public int? OperatorId { get; set; }
        public int? GameId { get; set; }
        public int? GameMathId { get; set; }
        public string UserId { get; set; }
        public string Feature { get; set; }
        public decimal Value { get; set; }
        public bool IsLiability { get; set; }
        public bool OperatorEnabled { get; set; }
        public bool UserEnabled { get; set; }
    }
}
