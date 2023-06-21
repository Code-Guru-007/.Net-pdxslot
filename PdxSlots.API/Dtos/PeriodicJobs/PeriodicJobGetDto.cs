namespace PdxSlots.API.Dtos.PeriodicJobs
{
    public class PeriodicJobGetDto
    {
        public int Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
        public IList<PeriodicJobZippedFileGetDto> ZippedFiles { get; set; }
    }
}
