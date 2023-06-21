namespace PdxSlots.API.Dtos.PeriodicJobs
{
    public class PeriodicJobZippedFileGetDto
    {
        public int Id { get; set; }
        public int PeriodicJobId { get; set; }
        public int ZippedFileId { get; set; }
        public string OriginalHash { get; set; }
        public string CurrentHash { get; set; }
        public bool HashEquals { get; set; }
        public bool HashCheck { get; set; }
        public DateTime Created { get; set; }
        public ZippedFile.ZippedFileGetDto ZippedFile { get; set; }
    }
}
