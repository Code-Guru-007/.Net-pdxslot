namespace PdxSlots.API.Dtos.Events
{
    public class EventPostDto
    {
        public int? GameId { get; set; }
        public int? GameMathId { get; set; }
        public int? OperatorId { get; set; }
        public int? RoundId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
