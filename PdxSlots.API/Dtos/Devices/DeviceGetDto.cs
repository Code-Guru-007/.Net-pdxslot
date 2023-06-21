namespace PdxSlots.API.Dtos.Devices
{
    public class DeviceGetDto
    {
        public int Id { get; set; }
        public string IpAddress { get; set; }
        public string OperatingSystem { get; set; }
        public string Browser { get; set; }
        public bool Mobile { get; set; }
        public DateTime Created { get; set; }
    }
}
