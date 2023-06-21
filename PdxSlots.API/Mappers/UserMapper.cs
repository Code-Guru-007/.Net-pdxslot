using AutoMapper;
using PdxSlots.API.Dtos.Users;

namespace PdxSlots.API.Mappers
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<Models.User, UserGetDto>();
            CreateMap<Models.User, AdminUserGetDto>();
            CreateMap<AdminUserPutDto, Models.User>();
        }
    }
}
