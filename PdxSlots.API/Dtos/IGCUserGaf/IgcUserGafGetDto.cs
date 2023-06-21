namespace PdxSlots.API.Dtos.IGCUserGaf
{
    public class IgcUserGafGetDto
    {
        public int Id { get; set; }
        public int OperatorId { get; set; }
        public string UserId { get; set; }
        public string Gaf { get; set; }
        public DateTime Created { get; set; }
    }
}
