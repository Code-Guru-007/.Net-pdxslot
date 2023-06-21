using AutoMapper;

namespace PdxSlots.API.Mappers
{
    public class DeviceMapper : Profile
    {
        public DeviceMapper()
        {
            CreateMap<Models.Device, Dtos.Devices.DeviceGetDto>();
        }
    }
}
